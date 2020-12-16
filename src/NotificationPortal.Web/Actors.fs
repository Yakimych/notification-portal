module NotificationPortal.Web.Actors

open Akka.Event
open Akka.FSharp
open Akka.Actor
open NotificationPortal.Web.Controllers
open NotificationPortal.Data
open NotificationPortal.Data.Core

type NotificationMessage =
    | ChallengeIssued of SendChallengeModel
    | ChallengeAccepted of int
    | ChallengeDeclined of int
    | ChallengeEntrySaved of ChallengeEntry
    | FirebaseNotificationSent of challengeEntry: ChallengeEntry * firebaseResponse: string
    | ChallengeStatusUpdated of challengeEntryId: int * newStatus: ChallengeStatus

type Message =
    | Subscribe
    | Unsubscribe
    | Msg of IActorRef * NotificationMessage

let setup () : IActorRef =
    let fakeSendToFirebase (challengeEntry: ChallengeEntry) : Async<NotificationMessage> =
        async {
            do! System.Threading.Tasks.Task.Delay(100) |> Async.AwaitTask
//            return "fakeFirebaseResponse"
            return FirebaseNotificationSent (challengeEntry, "fakeFirebaseResponse")
        }

    let fakeSendToFirebaseTask (challengeEntry: ChallengeEntry) : System.Threading.Tasks.Task<NotificationMessage> =
        let result = async {
            do! System.Threading.Tasks.Task.Delay(100) |> Async.AwaitTask
            return FirebaseNotificationSent (challengeEntry, "fakeFirebaseResponse")
        }
        result |> Async.StartAsTask

    use system = System.create "relogify-pub-sub-system" (Configuration.load())

    let firebaseSubscriber =
        spawn system "firebase-subscriber"
            (actorOf2 (fun mailbox msg ->
                let eventStream = mailbox.Context.System.EventStream
                match msg with
                | Msg (sender, content) ->
                    printfn "Firebase service received message from %A: %A" (sender.Path) content
                    match content with
                    | ChallengeEntrySaved challengeEntry ->
                        printf "Sending firebase notification for challengeId: %A to: %A" challengeEntry.Id challengeEntry.ToPlayer
                        fakeSendToFirebase challengeEntry |!> mailbox.Self
//                        FirebaseMessagingService.SendMessage()
                    | ChallengeAccepted challengeId ->
                        printf "Sending firebase notification to accept challenge with id: %A" challengeId
                    | ChallengeDeclined challengeId ->
                        printf "Sending firebase notification to decline challenge with id: %A" challengeId
                    | otherMessage ->
                        printf "No work to be performed for message: %A" otherMessage // TODO: UnhandledMessage?
                | Subscribe -> subscribe typeof<Message> mailbox.Self eventStream |> ignore
                | Unsubscribe -> unsubscribe typeof<Message> mailbox.Self eventStream |> ignore ))

    let publisher =
        spawn system "publisher"
            (actorOf2 (fun mailbox msg ->
                publish msg mailbox.Context.System.EventStream))

    firebaseSubscriber <! Subscribe
    publisher <! Msg (publisher, ChallengeAccepted 1)

    publisher
