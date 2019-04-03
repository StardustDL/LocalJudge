using System.Collections.Generic;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string NormalizedName { get; set; }

        public string NormalizedEmail { get; set; }
    }
}
