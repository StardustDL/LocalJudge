using StarOJ.Core;
using StarOJ.Core.Helpers;
using StarOJ.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class TestCaseProvider : ITestCaseProvider
    {
        public const string PF_Profile = "profile.json", PF_Input = "input.data", PF_Output = "output.data";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public string Input { get; private set; }

        public string Output { get; private set; }

        public Task<DataPreview> GetInputPreview(int maxbytes) => Task.FromResult(TextIO.GetPreviewInUTF8(Input, maxbytes));

        public async Task<TestCaseMetadata> GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCaseMetadata>(await TextIO.ReadAllInUTF8Async(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }


        public Task<DataPreview> GetOutputPreview(int maxbytes) => Task.FromResult(TextIO.GetPreviewInUTF8(Output, maxbytes));

        public Task<string> GetInput() => TextIO.ReadAllInUTF8Async(Input);

        public Task<string> GetOutput() => TextIO.ReadAllInUTF8Async(Output);

        public TestCaseProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Input = Path.Combine(Root, PF_Input);
            Output = Path.Combine(Root, PF_Output);
        }

        public static async Task<TestCaseProvider> Initialize(string root, TestCaseMetadata metadata = null, string input = "", string output = "")
        {
            var res = new TestCaseProvider(root);
            if (metadata == null) metadata = new TestCaseMetadata { TimeLimit = TimeSpan.FromSeconds(1), MemoryLimit = 128 * 1024 * 1024 };
            metadata.Id = res.Id;
            await TextIO.WriteAllInUTF8Async(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
            await TextIO.WriteAllInUTF8Async(res.Input, input);
            await TextIO.WriteAllInUTF8Async(res.Output, output);
            return res;
        }

        public Task SetMetadata(TestCaseMetadata value)
        {
            return TextIO.WriteAllInUTF8Async(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));
        }

        public Task SetInput(string value)
        {
            return TextIO.WriteAllInUTF8Async(Input, value);
        }

        public Task SetOutput(string value)
        {
            return TextIO.WriteAllInUTF8Async(Output, value);
        }
    }
}
