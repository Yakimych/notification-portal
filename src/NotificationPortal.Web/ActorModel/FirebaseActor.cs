using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using FirebaseAdmin.Messaging;
// using FirebaseAdmin;
// using Google.Apis.Auth.OAuth2;
using NotificationPortal.Data;

namespace NotificationPortal.Web.ActorModel
{
    public class FirebaseActor : ReceiveActor
    {
        private async Task<ChallengeNotification> FakeSendToFirebase(ChallengeEntry challengeEntry)
        {
            var encodedTopic = $"{challengeEntry.CommunityName}_{challengeEntry.ToPlayer}".Base64UrlEncode();
            var notificationMessage = $"{challengeEntry.CommunityName}: {challengeEntry.FromPlayer} has challenged you to a game!";

            var notificationTitle = "New Challenge!";
            var message = new Message
            {
                Notification = new Notification
                {
                    Title = notificationTitle,
                    Body = notificationMessage
                },
                Data = new Dictionary<string, string>
                {
                    { "challengeId", challengeEntry.Id.ToString() },
                    { "title", notificationTitle },
                    { "message", notificationMessage },
                    { "communityName", challengeEntry.CommunityName },
                    { "fromPlayer", challengeEntry.FromPlayer },
                    { "toPlayer", challengeEntry.ToPlayer },
                },
                Topic = encodedTopic
            };

            var challengeNotification = new ChallengeNotification
            {
                Topic = encodedTopic,
                Message = notificationMessage,
                FromPlayer = challengeEntry.FromPlayer,
                Date = DateTime.UtcNow,
                Type = NotificationType.Challenged // TODO: take in type as method parameter?
            };

            // TODO: Read configuration value and make firebase request
            // var notificationJsonString = _configuration["firebase_json"];
            // if (string.IsNullOrEmpty(notificationJsonString))
            // {
            //     return challengeNotification with { FirebaseResponse = "Mock response - Firebase configuration missing" };
            // }
            //
            // if (FirebaseApp.DefaultInstance == null)
            // {
            //     FirebaseApp.Create(new AppOptions
            //     {
            //         Credential = GoogleCredential.FromJson(notificationJsonString)
            //     });
            // }
            //
            // var messageResult = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            var messageResult = $"fakeFirebaseResponse: '{challengeEntry.Id}' '{Guid.NewGuid()}'";

            await Task.Delay(3000);

            return challengeNotification with { FirebaseResponse = messageResult };
        }

        public FirebaseActor()
        {
            Receive<ChallengeEntrySavedMessage>(message =>
            {
                var eventStream = Context.System.EventStream;
                FakeSendToFirebase(message.ChallengeEntry)
                    .ContinueWith(sendToFirebaseTask =>
                        eventStream.Publish(
                            new FirebaseNotificationSentMessage
                            {
                                ChallengeEntry = message.ChallengeEntry,
                                ChallengeNotification = sendToFirebaseTask.Result
                            }));
            });
        }
    }
}
