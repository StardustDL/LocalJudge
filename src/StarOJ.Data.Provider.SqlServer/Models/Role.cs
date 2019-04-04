using StarOJ.Core;
using StarOJ.Core.Identity;
using System.Collections.Generic;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }
}
