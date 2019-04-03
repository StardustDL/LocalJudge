using StarOJ.Core.Helpers;
using StarOJ.Core.Identity;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class RoleProvider : IRoleProvider
    {
        public const string PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public async Task<RoleMetadata> GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleMetadata>(await TextIO.ReadAllInUTF8Async(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public Task SetMetadata(RoleMetadata value)
        {
            return TextIO.WriteAllInUTF8Async(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
        }

        public RoleProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static async Task<RoleProvider> Initialize(string root, RoleMetadata metadata = null)
        {
            var res = new RoleProvider(root);
            if (metadata == null) metadata = new RoleMetadata() { Name = res.Id, NormalizedName = res.Id };
            metadata.Id = res.Id;
            await res.SetMetadata(metadata);
            return res;
        }
    }
}
