using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using NotificationPortal.Web.Data;

namespace NotificationPortal.Web.Core
{
    public class FirebaseMessagingService
    {
        private readonly IConfiguration _configuration;

        public FirebaseMessagingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ChallengeNotification> SendMessage(string communityName, string fromPlayer, string toPlayer)
        {
            var topic = $"{communityName}_{toPlayer}";
            var notificationMessage = $"{communityName}: {fromPlayer} has challenged you to a game!";

            var message = new Message
            {
                Data = new Dictionary<string, string>
                {
                    { "message", notificationMessage },
                    { "communityName", communityName },
                    { "fromPlayer", fromPlayer },
                    { "toPlayer", toPlayer }
                },
                Topic = topic,
                Notification = new Notification
                {
                    Title = "New challenge!",
                    Body = notificationMessage
                }
            };

            if (FirebaseApp.DefaultInstance == null)
            {
                var notificationJsonString = _configuration["firebase_json"];
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(notificationJsonString)
                });
            }

            var messageResult = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            // TODO: Returning a partial entry - is there a better way? Slimmed down Notification type?
            return new ChallengeNotification
            {
                Topic = topic,
                Message = notificationMessage,
                FromPlayer = fromPlayer,
                Date = DateTime.UtcNow,
                FirebaseResponse = messageResult,
                Type = NotificationType.Challenged // TODO: take in type as method parameter?
            };
        }

        public async Task<ChallengeNotification> SendMessageResponseToChallenge(
            string communityName, string respondingPlayer, string playerThatChallenged, NotificationType response)
        {
            var responseString = response.ToString().ToLower();

            var topic = $"{communityName}_{playerThatChallenged}";
            var notificationMessage =
                $"{communityName}: {playerThatChallenged} has {responseString} your challenge!";

            var message = new Message
            {
                Data = new Dictionary<string, string>
                {
                    { "message", notificationMessage },
                    { "communityName", communityName },
                    { "responseType", response.ToString() },
                    { "fromPlayer", respondingPlayer },
                    { "toPlayer", playerThatChallenged }
                },
                Topic = topic,
                Notification = new Notification
                {
                    Title = $"Challenge {responseString}",
                    Body = notificationMessage
                }
            };

            if (FirebaseApp.DefaultInstance == null)
            {
                var notificationJsonString = _configuration["firebase_json"];
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(notificationJsonString)
                });
            }

            var messageResult = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            // TODO: Returning a partial entry - is there a better way? Slimmed down Notification type?
            return new ChallengeNotification
            {
                Topic = topic,
                Message = notificationMessage,
                FromPlayer = respondingPlayer,
                Date = DateTime.UtcNow,
                FirebaseResponse = messageResult,
                Type = response
            };
        }
    }
}
