using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using NotificationPortal.Data;

namespace NotificationPortal.Web.Core
{
    public class FirebaseMessagingService
    {
        private readonly IConfiguration _configuration;

        public FirebaseMessagingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ChallengeNotification> SendMessage(ChallengeEntry challenge)
        {
            var encodedTopic = $"{challenge.CommunityName}_{challenge.ToPlayer}".Base64UrlEncode();
            var notificationMessage = $"{challenge.CommunityName}: {challenge.FromPlayer} has challenged you to a game!";

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
                    { "challengeId", challenge.Id.ToString() },
                    { "title", notificationTitle },
                    { "message", notificationMessage },
                    { "communityName", challenge.CommunityName },
                    { "fromPlayer", challenge.FromPlayer },
                    { "toPlayer", challenge.ToPlayer },
                },
                Topic = encodedTopic
            };

            var challengeNotification = new ChallengeNotification
            {
                Topic = encodedTopic,
                Message = notificationMessage,
                FromPlayer = challenge.FromPlayer,
                Date = DateTime.UtcNow,
                Type = NotificationType.Challenged // TODO: take in type as method parameter?
            };

            var notificationJsonString = _configuration["firebase_json"];
            if (string.IsNullOrEmpty(notificationJsonString))
            {
                return challengeNotification with { FirebaseResponse = "Mock response - Firebase configuration missing" };
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(notificationJsonString)
                });
            }

            var messageResult = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            // TODO: Returning a partial entry - is there a better way? Slimmed down Notification type?
            return challengeNotification with { FirebaseResponse = messageResult };
        }

        public async Task<ChallengeNotification> SendMessageResponseToChallenge(
            ChallengeEntry challenge, NotificationType response)
        {
            var responseString = response.ToString().ToLower();

            var playerThatChallenged = challenge.FromPlayer;
            var respondingPlayer = challenge.ToPlayer;

            var encodedTopic = $"{challenge.CommunityName}_{playerThatChallenged}".Base64UrlEncode();
            var notificationMessage =
                $"{challenge.CommunityName}: {respondingPlayer} has {responseString} your challenge!";

            var message = new Message
            {
                Data = new Dictionary<string, string>
                {
                    { "challengeId", challenge.Id.ToString() },
                    { "title", $"Challenge {responseString}" },
                    { "message", notificationMessage },
                    { "communityName", challenge.CommunityName },
                    { "responseType", response.ToString() },
                    { "fromPlayer", respondingPlayer },
                    { "toPlayer", playerThatChallenged }
                },
                Topic = encodedTopic
            };

            var challengeNotificaton = new ChallengeNotification
            {
                Topic = encodedTopic,
                Message = notificationMessage,
                FromPlayer = challenge.FromPlayer,
                Date = DateTime.UtcNow,
                Type = NotificationType.Challenged // TODO: take in type as method parameter?
            };

            var notificationJsonString = _configuration["firebase_json"];
            if (string.IsNullOrEmpty(notificationJsonString))
            {
                return challengeNotificaton with { FirebaseResponse = "Mock response - Firebase configuration missing" };
            }
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(notificationJsonString)
                });
            }

            var messageResult = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            // TODO: Returning a partial entry - is there a better way? Slimmed down Notification type?
            return challengeNotificaton with { FirebaseResponse = messageResult };
        }
    }
}