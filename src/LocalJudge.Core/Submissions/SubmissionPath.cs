﻿using LocalJudge.Core.Helpers;
using LocalJudge.Core.Judgers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Submissions
{
    public class SubmissionPath
    {
        static readonly Dictionary<ProgrammingLanguage, string> Extends = new Dictionary<ProgrammingLanguage, string>
        {
            [ProgrammingLanguage.C] = "c",
            [ProgrammingLanguage.Cpp] = "cpp",
            [ProgrammingLanguage.CSharp] = "cs",
            [ProgrammingLanguage.Java] = "java",
            [ProgrammingLanguage.Python] = "py",
        };

        public const string PF_Profile = "profile.json", PF_Code = "code", PF_Result = "result.json";

        public string Root { get; private set; }

        public string ID { get; private set; }

        public string Profile { get; private set; }

        public SubmissionMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<SubmissionMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.ID = Path.GetFileName(Root);
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

        public void SaveResult(SubmissionResult result)
        {
            TextIO.WriteAllInUTF8(Result, Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
        }

        public string Code
        {
            get
            {
                var meta = GetMetadata();
                return Path.Combine(Root, PF_Code + "." + Extends[meta.Language]);
            }
        }

        public string GetCode() => TextIO.ReadAllInUTF8(Code);

        public SubmissionPath(string root)
        {
            Root = root;
            ID = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Result = Path.Combine(Root, PF_Result);
        }

        public static SubmissionPath Initialize(string root, SubmissionMetadata metadata = null, string code = "")
        {
            var res = new SubmissionPath(root);
            if (metadata == null) metadata = new SubmissionMetadata();
            metadata.ID = res.ID;
            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
            TextIO.WriteAllInUTF8(res.Code, code);
            return res;
        }
    }
}