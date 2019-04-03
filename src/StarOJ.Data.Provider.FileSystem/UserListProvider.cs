using StarOJ.Core.Identity;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StarOJ.Data.Provider.FileSystem
{
    public class UserListProvider : IUserListProvider
    {
        public string Root { get; private set; }

        public IUserProvider Create(UserMetadata metadata)
        {
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            var res = UserProvider.Initialize(path, metadata);
            return res;
        }

        public IUserProvider Create(string id)
        {
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return UserProvider.Initialize(path);
        }

        public void Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
        }

        public IUserProvider Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Directory.Exists(path) ? new UserProvider(path) : null;
        }

        public IEnumerable<IUserProvider> GetAll() => Directory.GetDirectories(Root).Select(path => new UserProvider(path));

        public IUserProvider GetByName(string name)
        {
            return GetAll().FirstOrDefault(x => x.GetMetadata().NormalizedName == name);
        }

        public bool Has(string id) => Directory.Exists(Path.Combine(Root, id));

        public UserListProvider(string root)
        {
            Root = root;
        }
    }
}
