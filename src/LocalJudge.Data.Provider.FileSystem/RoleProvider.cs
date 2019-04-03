using LocalJudge.Core.Helpers;
using LocalJudge.Core.Identity;
using System;
using System.IO;
using System.Text;

namespace LocalJudge.Data.Provider.FileSystem
{
    public class RoleProvider : IRoleProvider
    {
        public const string PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public RoleMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public void SetMetadata(RoleMetadata value)
        {
            TextIO.WriteAllInUTF8(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
        }

        public RoleProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static RoleProvider Initialize(string root, RoleMetadata metadata = null)
        {
            var res = new RoleProvider(root);
            if (metadata == null) metadata = new RoleMetadata() { Name = res.Id, NormalizedName = res.Id };
            metadata.Id = res.Id;
            res.SetMetadata(metadata);
            return res;
        }
    }
}
