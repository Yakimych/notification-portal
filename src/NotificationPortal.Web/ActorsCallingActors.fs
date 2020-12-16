module NotificationPortal.Web.ActorsCallingActors

open Akka.FSharp
open Akka.Actor
open NotificationPortal.Web.Controllers
open NotificationPortal.Data

type NotificationMessage =
    | ChallengeIssued of SendChallengeModel
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
            return FirebaseNotificationSent (challengeEntry, "fakeFirebaseResponse")
        }

    use system = System.create "relogify-actor-system" (Configuration.load())

    let firebaseActor =
        spawn system "firebase-actor"
            (fun mailbox ->
                let rec loop() = actor {
                    let! maybeMessage = mailbox.Receive()

                    match maybeMessage with
                    | None -> ()
                    | Some message ->
                        printfn "Firebase service received message: %A" message
                        match message with
                        | ChallengeEntrySaved challengeEntry ->
                            printf "Sending firebase notification for challengeId: %A to: %A" challengeEntry.Id challengeEntry.ToPlayer
                            fakeSendToFirebase challengeEntry |!> mailbox.Self
                        | otherMessage ->
                            printf "No work to be performed for message: %A" otherMessage // TODO: UnhandledMessage?

                    return! loop()
                }
                loop())




    firebaseActor


