using StarOJ.Core.Identity;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class UserProvider : IUserProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;
        private readonly User _user;

        public string Id => _user.Id.ToString();

        public Task<UserMetadata> GetMetadata()
        {
            return Task.FromResult(new UserMetadata
            {
                Id = Id,
                Name = _user.Name,
                NormalizedName = _user.NormalizedName,
                Email = _user.Email,
                EmailConfirmed = _user.EmailConfirmed,
                NormalizedEmail = _user.NormalizedEmail,
                PasswordHash = _user.PasswordHash,
            });
        }

        public async Task SetMetadata(UserMetadata value)
        {
            _user.Name = value.Name;
            _user.NormalizedName = value.NormalizedName;
            _user.Email = value.Email;
            _user.EmailConfirmed = value.EmailConfirmed;
            _user.NormalizedEmail = value.NormalizedEmail;
            _user.PasswordHash = value.PasswordHash;
            await _context.SaveChangesAsync();
        }

        public UserProvider(Workspace workspace, OJContext context, User user)
        {
            _workspace = workspace;
            _context = context;
            _user = user;
        }
    }
}
