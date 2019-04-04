using StarOJ.Core.Helpers;
using StarOJ.Core.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class RoleListProvider : IRoleListProvider
    {
        public string Root { get; private set; }

        public async Task<IRoleProvider> Create(RoleMetadata metadata)
        {
            metadata.Id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await RoleProvider.Initialize(path, metadata);
        }

        public async Task<IRoleProvider> Create()
        {
            string id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await RoleProvider.Initialize(path);
        }

        public Task Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public Task<IRoleProvider> Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Task.FromResult(Directory.Exists(path) ? (IRoleProvider)new RoleProvider(path) : null);
        }

        public Task<IEnumerable<IRoleProvider>> GetAll() => Task.FromResult(Directory.GetDirectories(Root).Select(path => (IRoleProvider)new RoleProvider(path)));

        public async Task<IRoleProvider> GetByName(string name)
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

        public Task Clear()
        {
            foreach (var v in Directory.GetDirectories(Root))
                Directory.Delete(v, true);
            return Task.CompletedTask;
        }

        public RoleListProvider(string root)
        {
            Root = root;
        }
    }
}
