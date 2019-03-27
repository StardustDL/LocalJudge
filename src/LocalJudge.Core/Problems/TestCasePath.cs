using LocalJudge.Core.Helpers;
using System;
using System.IO;

namespace LocalJudge.Core.Problems
{
    public class TestCasePath
    {
        public const string PF_Profile = "profile.json", PF_Input = "input.data", PF_Output = "output.data";

        public string Root { get; private set; }

        public string Profile { get; private set; }

        public TestCaseMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCaseMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.ID = Path.GetFileName(Root);
            return res;
        }

        public string Input { get; private set; }

        public string GetInput() => TextIO.ReadAllInUTF8(Input);

        public string Output { get; set; }

        public string GetOutput() => TextIO.ReadAllInUTF8(Output);

        public TestCasePath(string root)
        {
            Root = root;
            Profile = Path.Combine(Root, PF_Profile);
            Input = Path.Combine(Root, PF_Input);
            Output = Path.Combine(Root, PF_Output);
        }

        public static TestCasePath Initialize(string root, TestCaseMetadata metadata = null, string input = "", string output = "")
        {
            var res = new TestCasePath(root);
            if (metadata == null) metadata = new TestCaseMetadata { TimeLimit = 1, MemoryLimit = 128 };
            metadata.ID = Path.GetFileName(root);
            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata));
            TextIO.WriteAllInUTF8(res.Input, input);
            TextIO.WriteAllInUTF8(res.Output, output);
            return res;
        }
    }
}
