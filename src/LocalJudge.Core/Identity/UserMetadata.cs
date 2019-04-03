using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace LocalJudge.Core.Identity
{
    public class UserMetadata : IHasId<string>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string NormalizedName { get; set; }

        public string NormalizedEmail { get; set; }

        public List<RoleMetadata> Roles { get; set; } = new List<RoleMetadata>();
    }
}
