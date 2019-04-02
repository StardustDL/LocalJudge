using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LocalJudge.Server.Host.Helpers
{
    public static class Authorizations
    {
        public const string Administrator = "Administrator";
    }

    public class RoleStore : IRoleStore<Role>
    {
        private readonly IHttpClientFactory clientFactory;

        public RoleStore(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
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

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
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

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);
            return await client.GetAsync(roleId);
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
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

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
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
