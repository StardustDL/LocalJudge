using StarOJ.Core.Helpers;
using StarOJ.Core.Submissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class SubmissionProvider : ISubmissionProvider
    {
        public const string PF_Profile = "profile.json", PF_Result = "result.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public string Result { get; private set; }

        public async Task<SubmissionMetadata> GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SubmissionMetadata>(await TextIO.ReadAllInUTF8Async(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public async Task<SubmissionResult> GetResult()
        {
            if (File.Exists(Result))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<SubmissionResult>(await TextIO.ReadAllInUTF8Async(Result));
            else
                return null;
        }

        public Task SetMetadata(SubmissionMetadata value) => TextIO.WriteAllInUTF8Async(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));

        public async Task SetResult(SubmissionResult value)
        {
            if(value == null)
            {
                File.Delete(Result);
            }
            else
            {
                await TextIO.WriteAllInUTF8Async(Result, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
            }
        }

        public SubmissionProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Result = Path.Combine(Root, PF_Result);
        }

        public static async Task<SubmissionProvider> Initialize(string root, SubmissionMetadata metadata = null)
        {
            var res = new SubmissionProvider(root);
            if (metadata == null) metadata = new SubmissionMetadata() { CodeLength = 0, Code = "" };
            metadata.Id = res.Id;
            await res.SetMetadata(metadata);
            return res;
        }
    }
}
