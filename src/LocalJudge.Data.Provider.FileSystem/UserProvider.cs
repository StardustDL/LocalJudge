using LocalJudge.Core.Helpers;
using LocalJudge.Core.Identity;
using System.Collections.Generic;
using System.IO;

namespace LocalJudge.Data.Provider.FileSystem
{
    public class UserProvider : IUserProvider
    {
        public const string PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public UserMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<UserMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public void SetMetadata(UserMetadata value)
        {
            TextIO.WriteAllInUTF8(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
        }

        public UserProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static UserProvider Initialize(string root, UserMetadata metadata = null)
        {
            var res = new UserProvider(root);
            if (metadata == null) metadata = new UserMetadata() { Name = res.Id, NormalizedName = res.Id };
            if (metadata.Roles == null) metadata.Roles = new List<RoleMetadata>();
            metadata.Id = res.Id;
            res.SetMetadata(metadata);
            return res;
        }
    }
}
