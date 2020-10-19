using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace NotificationPortal.Web.Core
{
    public class FirebaseMessagingService
    {
        private readonly IConfiguration _configuration;

        public FirebaseMessagingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> SendMessage(string communityName, string fromPlayer, string toPlayer)
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

            return FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
