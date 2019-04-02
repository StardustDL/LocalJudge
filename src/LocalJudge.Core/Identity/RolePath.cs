using LocalJudge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Identity
{
    public class RolePath : IHasRoot, IHasId<string>
    {
        public const string PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public Role GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<Role>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public void SetMetadata(Role metadata)
        {
            TextIO.WriteAllInUTF8(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
        }

        public RolePath(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static RolePath Initialize(string root, Role metadata = null)
        {
            var res = new RolePath(root);
            if (metadata == null) metadata = new Role() { Name = res.Id, NormalizedName = res.Id };
            metadata.Id = res.Id;
            res.SetMetadata(metadata);
            return res;
        }
    }
}
