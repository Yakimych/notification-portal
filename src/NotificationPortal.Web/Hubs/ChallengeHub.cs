using Microsoft.AspNetCore.SignalR;
using NotificationPortal.Web.ActorModel;

namespace NotificationPortal.Web.Hubs
{
    public class ChallengeHub : Hub
    {
        private readonly RelogifyActorModel _relogifyActorModel;

        // TODO: Can this be triggered via the strongly typed IHubContext?
        // https://docs.microsoft.com/en-us/aspnet/core/signalr/hubcontext?view=aspnetcore-3.1#inject-a-strongly-typed-hubcontext
        public ChallengeHub(RelogifyActorModel relogifyActorModel)
        {
            _relogifyActorModel = relogifyActorModel;
        }

        public void AcceptChallenge(int challengeId) =>
            _relogifyActorModel.PublishMessage(new ChallengeAcceptedMessage { ChallengeEntryId = challengeId });

        public void DeclineChallenge(int challengeId) =>
            _relogifyActorModel.PublishMessage(new ChallengeDeclinedMessage { ChallengeEntryId = challengeId });
    }
}
