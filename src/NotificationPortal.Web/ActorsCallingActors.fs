module NotificationPortal.Web.ActorsCallingActors

open Akka.FSharp
open Akka.Actor
open NotificationPortal.Web.Controllers
open NotificationPortal.Data

type ActorMessage =
    | ChallengeIssued of SendChallengeModel
    | ChallengeEntrySaved of ChallengeEntry
    | FirebaseNotificationSent of challengeEntryId: int * firebaseResponse: string
    | ChallengeStatusUpdated of challengeEntryId: int * newStatus: ChallengeStatus

let setup () : IActorRef =
    let fakeSendToFirebase (challengeEntry: ChallengeEntry) : Async<ActorMessage> =
        async {
            do! System.Threading.Tasks.Task.Delay(100) |> Async.AwaitTask
            return FirebaseNotificationSent (challengeEntry.Id, "fakeFirebaseResponse")
        }

    let fakeSaveNotificationToDatabase (challengeEntryId: int) (firebaseResponse: string) : Async<unit> =
        async {
            do! System.Threading.Tasks.Task.Delay(100) |> Async.AwaitTask
            return ()
        }

    let fakeAddChallengeEntryToDatabase (challengeModel: SendChallengeModel) : Async<ActorMessage> =
        async {
            do! System.Threading.Tasks.Task.Delay(100) |> Async.AwaitTask
            let fakeChallengeEntry =
                ChallengeEntry(Id = 1, CommunityName = "TestCommunity", FromPlayer = "From", ToPlayer = "To", Status = ChallengeStatus.Challenging, Date = System.DateTime.UtcNow)
            return ChallengeEntrySaved fakeChallengeEntry
        }

    let fakeUpdateChallengeStatusInDatabase (challengeEntryId: int) (newStatus: ChallengeStatus) : Async<ActorMessage> =
        async {
            do! System.Threading.Tasks.Task.Delay(100) |> Async.AwaitTask
            return ChallengeStatusUpdated (challengeEntryId, newStatus)
        }

    use system = System.create "relogify-actor-system" (Configuration.load())

    let signalRHandler (mailbox: Actor<ActorMessage>) (message: ActorMessage) =
        printfn "SignalR actor received message: %A" message
        match message with
        | ChallengeEntrySaved newChallengeEntry ->
            printf "Notifying SignalR hub subscribers - Challenge Entry Added: %A" newChallengeEntry
        | ChallengeStatusUpdated (challengeEntryId, newStatus) ->
            printf "Notifying SignalR hub subscribers - Challenge Status Changed: challengeEntryId: %A, newStatus: %A" challengeEntryId newStatus
        | unhandledMessage -> printf "No work to be performed for message: %A" unhandledMessage

    let signalRActor = spawn system "signalr-actor" (actorOf2 signalRHandler)

    let notificationMessageHandler (mailbox: Actor<ActorMessage>) (message: ActorMessage) =
        printfn "Notification actor received message: %A" message
        match message with
        | FirebaseNotificationSent (challengeEntryId, firebaseResponse) ->
            printf "Saving new notification to the database for challengeEntryId: %A, with firebaseResponse: %A" challengeEntryId firebaseResponse
            (fakeSaveNotificationToDatabase challengeEntryId firebaseResponse) |> Async.StartAsTask |> ignore // Fire and forget - is there a better way than StartAsTask?
        | unhandledMessage -> printf "No work to be performed for message: %A" unhandledMessage

    let notificationActor = spawn system "notification-actor" (actorOf2 notificationMessageHandler)

    let challengeUpdateHandler (mailbox: Actor<ActorMessage>) (message: ActorMessage) =
        printfn "Challenge update actor received message: %A" message
        match message with
        | FirebaseNotificationSent (challengeEntryId, _) ->
            let newChallengeStatus = ChallengeStatus.Challenged
            printf "Updating status for challengeEntryId %A, to %A" challengeEntryId newChallengeStatus
            (fakeUpdateChallengeStatusInDatabase challengeEntryId newChallengeStatus) |!> signalRActor
        | unhandledMessage -> printf "No work to be performed for message: %A" unhandledMessage

    let challengeActor = spawn system "challenge-actor" (actorOf2 challengeUpdateHandler)

    let firebaseMessageHandler (mailbox: Actor<ActorMessage>) (message: ActorMessage) =
        printfn "Firebase actor received message: %A" message
        match message with
        | ChallengeEntrySaved challengeEntry ->
            printf "Sending firebase notification for challengeId: %A to: %A" challengeEntry.Id challengeEntry.ToPlayer
            let sendToFirebaseResult = fakeSendToFirebase challengeEntry
            sendToFirebaseResult |!> notificationActor
            sendToFirebaseResult |!> challengeActor
        | unhandledMessage -> printf "No work to be performed for message: %A" unhandledMessage

    let firebaseActor = spawn system "firebase-actor" (actorOf2 firebaseMessageHandler)

    let challengeCreationHandler (mailbox: Actor<ActorMessage>) (message: ActorMessage) =
        printfn "Challenge creation actor received message: %A" message
        match message with
        | ChallengeIssued challengeModel ->
            printf "Saving new challengeEntry to the database: %A" challengeModel
            let databaseAddingAsyncOperation = fakeAddChallengeEntryToDatabase challengeModel
            databaseAddingAsyncOperation |!> firebaseActor
            databaseAddingAsyncOperation |!> signalRActor
        | unhandledMessage -> printf "No work to be performed for message: %A" unhandledMessage

    let challengeCreationActor = spawn system "challenge-creation-actor" (actorOf2 challengeCreationHandler)

    challengeCreationActor


