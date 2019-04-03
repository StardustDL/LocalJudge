using LocalJudge.Core.Helpers;
using LocalJudge.Core.Submissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Data.Provider.FileSystem
{
    public class SubmissionProvider : ISubmissionProvider
    {
        public const string PF_Profile = "profile.json", PF_Result = "result.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public string Result { get; private set; }

        string GetCodePath()
        {
            var meta = GetMetadata();
            return Path.Combine(Root, meta.CodeName);
        }

        public string GetCode() => TextIO.ReadAllInUTF8(GetCodePath());

        public SubmissionMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SubmissionMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public SubmissionResult GetResult()
        {
            if (File.Exists(Result))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<SubmissionResult>(TextIO.ReadAllInUTF8(Result));
            else
                return null;
        }

        public void SetCode(string value) => TextIO.WriteAllInUTF8(GetCodePath(), value);

        public void SetMetadata(SubmissionMetadata value) => TextIO.WriteAllInUTF8(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));

        public void SetResult(SubmissionResult value)
        {
            if(value == null)
            {
                File.Delete(Result);
            }
            else
            {
                TextIO.WriteAllInUTF8(Result, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
            }
        }

        public SubmissionProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Result = Path.Combine(Root, PF_Result);
        }

        public static SubmissionProvider Initialize(string root, SubmissionMetadata metadata = null, string code = "")
        {
            var res = new SubmissionProvider(root);
            if (metadata == null) metadata = new SubmissionMetadata() { CodeLength = 0, CodeName = "code.txt" };
            metadata.Id = res.Id;
            res.SetMetadata(metadata);
            res.SetCode(code);
            return res;
        }
    }
}
