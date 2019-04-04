using StarOJ.Core.Identity;
using StarOJ.Data.Provider.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class RoleProvider : IRoleProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;
        private readonly Role _role;

        public string Id => _role.Id.ToString();

        public Task<RoleMetadata> GetMetadata()
        {
            return Task.FromResult(new RoleMetadata
            {
                Id = Id,
                Name = _role.Name,
                NormalizedName = _role.NormalizedName,
            });
        }

        public async Task SetMetadata(RoleMetadata value)
        {
            _role.Name = value.Name;
            _role.NormalizedName = value.NormalizedName;
            await _context.SaveChangesAsync();
        }

        public RoleProvider(Workspace workspace, OJContext context, Role role)
        {
            _workspace = workspace;
            _context = context;
            _role = role;
        }
    }
}
