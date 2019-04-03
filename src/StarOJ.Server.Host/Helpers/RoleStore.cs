using StarOJ.Server.Host.APIClients;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Helpers
{
    public static class Authorizations
    {
        public const string Administrator = "Administrator";
    }

    public class RoleStore : IRoleStore<RoleMetadata>
    {
        private readonly IHttpClientFactory clientFactory;

        public RoleStore(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<IdentityResult> CreateAsync(RoleMetadata role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);
            try
            {
                await client.CreateAsync(role);
                return IdentityResult.Success;
            }
            catch
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Creating role {role.Name} failed." });
            }
        }

        public async Task<IdentityResult> DeleteAsync(RoleMetadata role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);
            try
            {
                await client.DeleteAsync(role.Id);
                return IdentityResult.Success;
            }
            catch
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Deleting role {role.Name} failed." });
            }
        }

        public void Dispose()
        {
            
        }

        public async Task<RoleMetadata> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);
            return await client.GetAsync(roleId);
        }

        public async Task<RoleMetadata> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);
            try
            {
                return await client.GetByNameAsync(normalizedRoleName);
            }
            catch
            {
                return null;
            }
        }

        public Task<string> GetNormalizedRoleNameAsync(RoleMetadata role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(RoleMetadata role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(RoleMetadata role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(RoleMetadata role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(RoleMetadata role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(RoleMetadata role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);
            try
            {
                await client.UpdateAsync(role);
                return IdentityResult.Success;
            }
            catch
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Updating role {role.Name} failed." });
            }
        }
    }
}
