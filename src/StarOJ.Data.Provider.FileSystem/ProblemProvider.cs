using StarOJ.Core.Helpers;
using StarOJ.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    class ProblemDescriptionHelper
    {
        public const string PF_Description = "description.md", PF_Input = "input.md", PF_Output = "output.md", PF_Hint = "hint.md";

        public static async Task<ProblemDescription> LoadFromPlainText(string root)
        {
            string description = Path.Combine(root, PF_Description);
            string input = Path.Combine(root, PF_Input);
            string output = Path.Combine(root, PF_Output);
            string hint = Path.Combine(root, PF_Hint);
            return new ProblemDescription
            {
                Description = await TextIO.ReadAllInUTF8Async(description),
                Input = await TextIO.ReadAllInUTF8Async(input),
                Output = await TextIO.ReadAllInUTF8Async(output),
                Hint = await TextIO.ReadAllInUTF8Async(hint)
            };
        }

        public static async Task Initialize(string root)
        {
            string description = Path.Combine(root, PF_Description);
            string input = Path.Combine(root, PF_Input);
            string output = Path.Combine(root, PF_Output);
            string hint = Path.Combine(root, PF_Hint);

            await TextIO.WriteAllInUTF8Async(description, string.Empty);
            await TextIO.WriteAllInUTF8Async(input, string.Empty);
            await TextIO.WriteAllInUTF8Async(output, string.Empty);
            await TextIO.WriteAllInUTF8Async(hint, string.Empty);
        }

        public static async Task Extract(string root, ProblemDescription value)
        {
            string description = Path.Combine(root, PF_Description);
            string input = Path.Combine(root, PF_Input);
            string output = Path.Combine(root, PF_Output);
            string hint = Path.Combine(root, PF_Hint);

            await TextIO.WriteAllInUTF8Async(description, value.Description);
            await TextIO.WriteAllInUTF8Async(input, value.Input);
            await TextIO.WriteAllInUTF8Async(output, value.Output);
            await TextIO.WriteAllInUTF8Async(hint, value.Hint);
        }
    }

    public class ProblemProvider : IProblemProvider
    {
        public const string PF_Profile = "profile.json";
        public const string PD_Description = "description", PD_Sample = "samples", PD_Test = "tests", PD_Extra = "extra";

        public string Root { get; private set; }

        public string Id { get; private set; }

        public string Profile { get; private set; }

        public string Description { get; private set; }

        public ITestCaseListProvider Samples { get; private set; }

        public ITestCaseListProvider Tests { get; private set; }

        public Task<ProblemDescription> GetDescription() => ProblemDescriptionHelper.LoadFromPlainText(Description);

        public async Task<ProblemMetadata> GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemMetadata>(await TextIO.ReadAllInUTF8Async(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public Task SetMetadata(ProblemMetadata value) => TextIO.WriteAllInUTF8Async(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));

        public ProblemProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Description = Path.Combine(Root, PD_Description);
            Samples = new TestCaseListProvider(Path.Combine(Root, PD_Sample));
            Tests = new TestCaseListProvider(Path.Combine(Root, PD_Test));
        }

        public static async Task<ProblemProvider> Initialize(string root, ProblemMetadata metadata = null, ProblemDescription description = null)
        {
            var res = new ProblemProvider(root);
            if (metadata == null) metadata = new ProblemMetadata { UserId = "", Name = "Untitled", Source = "" };
            metadata.Id = res.Id;

            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
            Directory.CreateDirectory(res.Description);
            if (description == null)
                await ProblemDescriptionHelper.Initialize(res.Description);
            else
                await ProblemDescriptionHelper.Extract(res.Description, description);

            Directory.CreateDirectory((res.Samples as TestCaseListProvider).Root);
            await res.Samples.Create();

            Directory.CreateDirectory((res.Tests as TestCaseListProvider).Root);
            await res.Tests.Create();

            return res;
        }

        public Task SetDescription(ProblemDescription value)
        {
            return ProblemDescriptionHelper.Extract(Description, value);
        }
    }
}
