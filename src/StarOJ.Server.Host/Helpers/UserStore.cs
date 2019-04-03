using StarOJ.Server.Host.APIClients;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Helpers
{
    public class UserStore : IUserStore<UserMetadata>, IUserPasswordStore<UserMetadata>, IUserEmailStore<UserMetadata>
    {
        private readonly IHttpClientFactory clientFactory;

        public UserStore(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        /*public async Task AddToRoleAsync(UserMetadata user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new RolesClient(httpclient);

            var role = await client.GetByNameAsync(roleName);
            var index = user.Roles.ToList().FindIndex(x => x.Id == role.Id);
            if (index != -1) return;
            user.Roles.Add(role);
        }

            public Task<IList<string>> GetRolesAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user.Roles == null)
                return Task.FromResult<IList<string>>(Array.Empty<string>());
            return Task.FromResult<IList<string>>(user.Roles.Select(x => x.Name).ToList());
        }
        public Task<bool> IsInRoleAsync(UserMetadata user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            if (user.Roles == null)
                return Task.FromResult(false);
            return Task.FromResult(user.Roles.Any(x => x.NormalizedName == roleName));
        }

        public Task RemoveFromRoleAsync(UserMetadata user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rs = user.Roles.ToList();
            var index = rs.FindIndex(x => x.NormalizedName == roleName);
            if (index == -1) return Task.CompletedTask;

            user.Roles.RemoveAt(index);
            return Task.CompletedTask;
        }
        
        public Task<IList<UserMetadata>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }*/

        public async Task<IdentityResult> CreateAsync(UserMetadata user, CancellationToken cancellationToken = default)
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

        public async Task<IdentityResult> DeleteAsync(UserMetadata user, CancellationToken cancellationToken)
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

        public Task<UserMetadata> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserMetadata> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpclient = clientFactory.CreateClient();
            var client = new UsersClient(httpclient);
            return await client.GetAsync(userId);
        }

        public async Task<UserMetadata> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
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

        public Task<string> GetEmailAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task<string> GetNormalizedUserNameAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedName);
        }

        public Task<string> GetPasswordHashAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Name);
        }

        public Task<bool> HasPasswordAsync(UserMetadata user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetEmailAsync(UserMetadata user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Email = email;

            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(UserMetadata user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(UserMetadata user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(UserMetadata user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(UserMetadata user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(UserMetadata user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Name = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(UserMetadata user, CancellationToken cancellationToken)
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
