using System.ComponentModel.DataAnnotations;

namespace NotificationPortal.Web.Models
{
    public record SendChallengeModel(
        [Required] string CommunityName,
        [Required] string FromPlayer,
        [Required] string ToPlayer,
        string? RequestStatusMessage);
}
