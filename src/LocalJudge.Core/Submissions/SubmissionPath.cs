using LocalJudge.Core.Helpers;
using LocalJudge.Core.Judgers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Submissions
{
    public class SubmissionPath : IHasRoot, IHasId<string>
    {
        public const string PF_Profile = "profile.json", PF_Result = "result.json";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public SubmissionMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SubmissionMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public string Result { get; private set; }

        public SubmissionResult GetResult()
        {
            if (File.Exists(Result))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<SubmissionResult>(TextIO.ReadAllInUTF8(Result));
            else
                return null;
        }

        public void SetResult(SubmissionResult result)
        {
            TextIO.WriteAllInUTF8(Result, Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
        }

        public void ClearResult()
        {
            File.Delete(Result);
        }

        public string GetCodePath()
        {
            var meta = GetMetadata();
            return Path.Combine(Root, meta.CodePath);
        }

        public string GetCode() => TextIO.ReadAllInUTF8(GetCodePath());

        public void SaveCode(string code) => TextIO.WriteAllInUTF8(GetCodePath(), code);

        public SubmissionPath(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Result = Path.Combine(Root, PF_Result);
        }

        public static SubmissionPath Initialize(string root, SubmissionMetadata metadata = null, string code = "")
        {
            var res = new SubmissionPath(root);
            if (metadata == null) metadata = new SubmissionMetadata() { CodeLength = 0, CodePath = "code.txt" };
            metadata.Id = res.Id;
            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
            TextIO.WriteAllInUTF8(res.GetCodePath(), code);
            return res;
        }
    }
}
