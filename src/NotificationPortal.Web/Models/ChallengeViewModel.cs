using System.ComponentModel.DataAnnotations;

namespace NotificationPortal.Web.Models
{
    public class SendChallengeModel
    {
        [Required]
        public string CommunityName { get; set; }

        [Required]
        public string FromPlayer { get; set; }

        [Required]
        public string ToPlayer { get; set; }

        public string TopicOverride { get; set; }

        public string RequestStatusMessage { get; set; }
    }
}
