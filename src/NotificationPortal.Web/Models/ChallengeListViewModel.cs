using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NotificationPortal.Data;
// using Swashbuckle.AspNetCore.Annotations;

namespace NotificationPortal.Web.Models
{
    // TODO: Use SwaggerSchema instead after this issue is resolved: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1920
    // [SwaggerSchema(Required = new[] { nameof(Id), nameof(ToPlayer) })]
    public record ChallengeModel(
        [Required] int Id,
        [Required] string CommunityName,
        [Required] string FromPlayer,
        [Required] string ToPlayer,
        [Required] ChallengeStatus Status,
        [Required] DateTime Date);

    public record ChallengeCollectionModel([Required] List<ChallengeModel> Challenges);
}
