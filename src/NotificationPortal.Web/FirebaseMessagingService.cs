using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using NotificationPortal.Data;

namespace NotificationPortal.Web
{
    public class FirebaseMessagingService
    {
        public FirebaseMessagingService(string firebaseConfigurationJsonString)
        {
            if (string.IsNullOrEmpty(firebaseConfigurationJsonString))
                throw new Exception(
                    $"Cannot instantiate {nameof(FirebaseMessagingService)}: Firebase configuration cannot be empty");

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                    { Credential = GoogleCredential.FromJson(firebaseConfigurationJsonString) });
            }
        }

        public async Task<ChallengeNotification> SendMessageWithInitialChallenge(ChallengeEntry challenge)
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

            var firebaseResponse = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            return new ChallengeNotification
            {
                Topic = encodedTopic,
                Message = notificationMessage,
                FromPlayer = challenge.FromPlayer,
                Date = DateTime.UtcNow, // TODO: Take in date as a method parameter
                Type = NotificationType.Challenged,
                FirebaseResponse = firebaseResponse
            };
        }

        public async Task<ChallengeNotification> SendMessageWithResponseToChallenge(
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

            var firebaseResponse = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            return new ChallengeNotification
            {
                Topic = encodedTopic,
                Message = notificationMessage,
                FromPlayer = challenge.FromPlayer,
                Date = DateTime.UtcNow, // TODO: Take in date as a method parameter
                Type = response,
                FirebaseResponse = firebaseResponse
            };
        }
    }
}
