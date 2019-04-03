using LocalJudge.Core;
using LocalJudge.Core.Helpers;
using LocalJudge.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Data.Provider.FileSystem
{
    public class TestCaseProvider : ITestCaseProvider
    {
        public const string PF_Profile = "profile.json", PF_Input = "input.data", PF_Output = "output.data";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public string Input { get; private set; }

        public string Output { get; private set; }

        public DataPreview GetInputPreview(int maxbytes) => TextIO.GetPreviewInUTF8(Input, maxbytes);

        public TestCaseMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCaseMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }


        public DataPreview GetOutputPreview(int maxbytes) => TextIO.GetPreviewInUTF8(Output, maxbytes);

        public string GetInput() => TextIO.ReadAllInUTF8(Input);

        public string GetOutput() => TextIO.ReadAllInUTF8(Output);

        public TestCaseProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Input = Path.Combine(Root, PF_Input);
            Output = Path.Combine(Root, PF_Output);
        }

        public static TestCaseProvider Initialize(string root, TestCaseMetadata metadata = null, string input = "", string output = "")
        {
            var res = new TestCaseProvider(root);
            if (metadata == null) metadata = new TestCaseMetadata { TimeLimit = TimeSpan.FromSeconds(1), MemoryLimit = 128 * 1024 * 1024 };
            metadata.Id = res.Id;
            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
            TextIO.WriteAllInUTF8(res.Input, input);
            TextIO.WriteAllInUTF8(res.Output, output);
            return res;
        }
    }
}
