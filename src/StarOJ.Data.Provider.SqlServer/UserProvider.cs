using StarOJ.Core.Identity;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class UserProvider : IUserProvider
    {
        private readonly OJContext _context;
        private readonly User _user;

        public string Id => _user.Id;

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
            _user.EmailConfirmed = _user.EmailConfirmed;
            _user.NormalizedEmail = _user.NormalizedEmail;
            _user.PasswordHash = _user.PasswordHash;
            await _context.SaveChangesAsync();
        }

        public UserProvider(OJContext context, User user)
        {
            _context = context;
            _user = user;
        }
    }
}
