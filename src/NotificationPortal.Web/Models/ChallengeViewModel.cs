using System.ComponentModel.DataAnnotations;

namespace NotificationPortal.Web.Models
{
    public class SendChallengeModel
    {
        [Required]
        public string CommunityName { get; init; }

        [Required]
        public string FromPlayer { get; init; }

        [Required]
        public string ToPlayer { get; init; }
    }
}
