using LocalJudge.Core.Helpers;
using LocalJudge.Core.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Data.Provider.FileSystem
{
    public class RoleListProvider : IRoleListProvider
    {
        public string Root { get; private set; }

        public IRoleProvider Create(RoleMetadata metadata)
        {
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            var res = RoleProvider.Initialize(path, metadata);
            return res;
        }

        public IRoleProvider Create(string id)
        {
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return RoleProvider.Initialize(path);
        }

        public void Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
        }

        public IRoleProvider Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Directory.Exists(path) ? new RoleProvider(path) : null;
        }

        public IEnumerable<IRoleProvider> GetAll() => Directory.GetDirectories(Root).Select(path => new RoleProvider(path));

        public IRoleProvider GetByName(string name)
        {
            return GetAll().FirstOrDefault(x => x.GetMetadata().NormalizedName == name);
        }

        public bool Has(string id) => Directory.Exists(Path.Combine(Root, id));

        public RoleListProvider(string root)
        {
            Root = root;
        }
    }
}
