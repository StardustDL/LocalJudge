using StarOJ.Core.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class UserListProvider : IUserListProvider
    {
        public string Root { get; private set; }

        public async Task<IUserProvider> Create(UserMetadata metadata)
        {
            metadata.Id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await UserProvider.Initialize(path, metadata);
        }

        public async Task<IUserProvider> Create()
        {
            string id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await UserProvider.Initialize(path);
        }

        public Task Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public Task<IUserProvider> Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Task.FromResult(Directory.Exists(path) ? (IUserProvider)new UserProvider(path) : null);
        }

        public Task<IEnumerable<IUserProvider>> GetAll() => Task.FromResult(Directory.GetDirectories(Root).Select(path => (IUserProvider)new UserProvider(path)));

        public async Task<IUserProvider> GetByName(string name)
        {
            var all = await GetAll();
            foreach (var v in all)
            {
                if (string.Equals((await v.GetMetadata()).Name, name, StringComparison.OrdinalIgnoreCase))
                    return v;
            }
            return null;
        }

        public Task<bool> Has(string id) => Task.FromResult(Directory.Exists(Path.Combine(Root, id)));

        public UserListProvider(string root)
        {
            Root = root;
        }
    }
}
