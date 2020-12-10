using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// using Swashbuckle.AspNetCore.Annotations;

namespace NotificationPortal.Web.Models
{
    // TODO: Use SwaggerSchema instead
    // [SwaggerSchema(Required = new[] { nameof(Id), nameof(ToPlayer) })]
    public class ChallengeModel
    {
        [Required]
        public int Id { get; init; }

        [Required]
        public string CommunityName { get; init; }

        [Required]
        public string FromPlayer { get; init; }

        [Required]
        public string ToPlayer { get; init; }

        [Required]
        public string Type { get; init; }

        [Required]
        public DateTime Date { get; init; }
    }

    public class ChallengeCollectionModel
    {
        [Required]
        public List<ChallengeModel> Challenges { get; init; }
    }
}
