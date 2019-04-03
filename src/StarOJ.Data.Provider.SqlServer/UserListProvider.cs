using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarOJ.Core.Identity;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Linq;

namespace StarOJ.Data.Provider.SqlServer
{
    public class UserListProvider : IUserListProvider
    {
        private readonly OJContext _context;

        public UserListProvider(OJContext context)
        {
            _context = context;
        }

        public async Task<IUserProvider> Create(UserMetadata metadata)
        {
            User empty = new User();
            _context.Users.Add(empty);
            await _context.SaveChangesAsync();
            var res = new UserProvider(_context, empty);
            await res.SetMetadata(metadata);
            return res;
        }

        public Task<IUserProvider> Create()
        {
            string name = Guid.NewGuid().ToString();
            return Create(new UserMetadata { Name = name, NormalizedName = name });
        }

        public async Task Delete(string id)
        {
            var item = await _context.Users.FindAsync(id);
            if (item != null)
            {
                _context.Users.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IUserProvider> Get(string id)
        {
            var item = await _context.Users.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            else
            {
                return new UserProvider(_context, item);
            }
        }

        public Task<IEnumerable<IUserProvider>> GetAll()
        {
            List<IUserProvider> res = new List<IUserProvider>();
            foreach (var v in _context.Users)
            {
                res.Add(new UserProvider(_context, v));
            }
            return Task.FromResult((IEnumerable<IUserProvider>)res);
        }

        public Task<IUserProvider> GetByName(string name)
        {
            var item = (from x in _context.Users where x.NormalizedName == name select x).FirstOrDefault();
            if (item == null)
                return Task.FromResult<IUserProvider>(null);
            else
                return Task.FromResult((IUserProvider)new UserProvider(_context, item));
        }

        public async Task<bool> Has(string id)
        {
            return await _context.Users.FindAsync(id) != null;
        }
    }

}
