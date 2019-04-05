using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text;

namespace StarOJ.Core.Identity
{
    public class UserMetadata : IHasId<string>
    {
        [Required]
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
