using StarOJ.Core.Helpers;
using StarOJ.Core.Identity;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class UserProvider : IUserProvider
    {
        public const string PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public async Task<UserMetadata> GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<UserMetadata>(await TextIO.ReadAllInUTF8Async(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public Task SetMetadata(UserMetadata value)
        {
            return TextIO.WriteAllInUTF8Async(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
        }

        public UserProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static async Task<UserProvider> Initialize(string root, UserMetadata metadata = null)
        {
            var res = new UserProvider(root);
            if (metadata == null) metadata = new UserMetadata() { Name = res.Id, NormalizedName = res.Id };
            metadata.Id = res.Id;
            await res.SetMetadata(metadata);
            return res;
        }
    }
}
