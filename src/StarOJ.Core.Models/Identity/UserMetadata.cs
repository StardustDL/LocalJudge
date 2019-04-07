using System.ComponentModel.DataAnnotations;

namespace StarOJ.Core.Identity
{
    public class UserMetadata : IHasId<string>
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string NormalizedName { get; set; }

        public string NormalizedEmail { get; set; }
    }
}
