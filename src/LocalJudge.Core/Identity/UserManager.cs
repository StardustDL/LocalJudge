using LocalJudge.Core.Helpers;
using LocalJudge.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Identity
{
    public class UserManager : IHasRoot, IPathItemManager<UserPath>
    {
        string GetNamePath(string name)
        {
            return Path.Combine(Root, $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(name))}.txt");
        }

        public string Root { get; private set; }

        public UserPath Create(string id, User metadata = null)
        {
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            var res = UserPath.Initialize(path, metadata);

            metadata = res.GetMetadata();
            TextIO.WriteAllInUTF8(GetNamePath(metadata.NormalizedName), res.Id);

            return res;
        }

        public UserPath Create(string id) => Create(id, null);

        public void Delete(string id)
        {
            var user = Get(id);
            if (user == null) return;
            var meta = user.GetMetadata();
            File.Delete(GetNamePath(meta.NormalizedName));
            Directory.Delete(user.Root, true);
        }

        public bool Update(User metadata)
        {
            var up = Get(metadata.Id);
            if (up == null) return false;
            var oldmeta = up.GetMetadata();
            if(oldmeta.NormalizedName != metadata.NormalizedName) // UserName changed
            {
                File.Delete(GetNamePath(oldmeta.NormalizedName));
                TextIO.WriteAllInUTF8(GetNamePath(metadata.NormalizedName), up.Id);
            }
            up.SetMetadata(metadata);
            return true;
        }

        public UserPath Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Directory.Exists(path) ? new UserPath(path) : null;
        }

        public UserPath GetByName(string name)
        {
            var npath = GetNamePath(name);
            if (!File.Exists(npath)) return null;
            return Get(TextIO.ReadAllInUTF8(npath));
        }

        public IEnumerable<UserPath> GetAll() => Directory.GetDirectories(Root).Select(path => new UserPath(path));

        public bool Has(string id) => Directory.Exists(Path.Combine(Root, id));

        public UserManager(string root) => Root = root;
    }
}
