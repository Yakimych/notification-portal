module NotificationPortal.Web.Actors

open Akka.FSharp
open Akka.Actor
open NotificationPortal.Web.Controllers
open NotificationPortal.Data.Core

type NotificationMessage =
    | NewChallenge of SendChallengeModel
    | ChallengeAccepted of int
    | ChallengeDeclined of int

type Message =
    | Subscribe
    | Unsubscribe
    | Msg of IActorRef * NotificationMessage

let setup () : IActorRef =
    use system = System.create "relogify-actor-system" (Configuration.load())

    let firebaseSubscriber =
        spawn system "firebase-subscriber"
            (actorOf2 (fun mailbox msg ->
                let eventStream = mailbox.Context.System.EventStream
                match msg with
                | Msg (sender, content) ->
                    printfn "Firebase service received message from %A: %A" (sender.Path) content
                    match content with
                    | NewChallenge sendChallengeModel ->
                        printf "Sending firebase notification to: %A" sendChallengeModel.ToPlayer
//                        FirebaseMessagingService.SendMessage()
                    | ChallengeAccepted challengeId ->
                        printf "Sending firebase notification to accept challenge with id: %A" challengeId
                    | ChallengeDeclined challengeId ->
                        printf "Sending firebase notification to decline challenge with id: %A" challengeId
                | Subscribe -> subscribe typeof<Message> mailbox.Self eventStream |> ignore
                | Unsubscribe -> unsubscribe typeof<Message> mailbox.Self eventStream |> ignore ))

    let publisher =
        spawn system "publisher"
            (actorOf2 (fun mailbox msg ->
                publish msg mailbox.Context.System.EventStream))

    firebaseSubscriber<! Subscribe
//    publisher <! Msg (publisher, ChallengeAccepted 1)

    publisher
