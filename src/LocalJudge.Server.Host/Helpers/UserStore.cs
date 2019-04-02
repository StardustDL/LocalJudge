using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LocalJudge.Server.Host.Helpers
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    {
        private readonly IHttpClientFactory clientFactory;

        public UserStore(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);

            var role = await client.GetByNameAsync(roleName);
            var index = user.Roles.ToList().FindIndex(x => x.Id == role.Id);
            if (index != -1) return;
            user.Roles.Add(role);
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            try
            {
                await client.CreateAsync(user);
                return IdentityResult.Success;
            }
            catch
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Creating user {user.Name} failed." });
            }
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            try
            {
                await client.DeleteAsync(user.Id);
                return IdentityResult.Success;
            }
            catch
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Deleting user {user.Name} failed." });
            }
        }

        public void Dispose()
        {

        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            return await client.GetAsync(userId);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            try
            {
                return await client.GetByNameAsync(normalizedUserName);
            }
            catch
            {
                return null;
            }
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedName);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash);
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user.Roles == null)
                return Task.FromResult<IList<string>>(Array.Empty<string>());
            return Task.FromResult<IList<string>>(user.Roles.Select(x => x.Name).ToList());
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Name);
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            if (user.Roles == null)
                return Task.FromResult(false);
            return Task.FromResult(user.Roles.Any(x => x.NormalizedName == roleName));
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rs = user.Roles.ToList();
            var index = rs.FindIndex(x => x.NormalizedName == roleName);
            if (index == -1) return Task.CompletedTask;

            user.Roles.RemoveAt(index);
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Name = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            try
            {
                await client.UpdateAsync(user);
                return IdentityResult.Success;
            }
            catch
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Updating user {user.Name} failed." });
            }
        }
    }
}
