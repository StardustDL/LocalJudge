using LocalJudge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Identity
{
    public class UserPath : IHasRoot,IHasId<string>
    {
        public const string PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public User GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public void SetMetadata(User metadata)
        {
            TextIO.WriteAllInUTF8(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
        }

        public UserPath(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static UserPath Initialize(string root, User metadata = null)
        {
            var res = new UserPath(root);
            if (metadata == null) metadata = new User() { Name = res.Id, NormalizedName = res.Id };
            if (metadata.Roles == null) metadata.Roles = new List<Role>();
            metadata.Id = res.Id;
            res.SetMetadata(metadata);
            return res;
        }
    }
}
