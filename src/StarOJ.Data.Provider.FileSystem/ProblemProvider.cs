using StarOJ.Core.Helpers;
using StarOJ.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StarOJ.Data.Provider.FileSystem
{
    class ProblemDescriptionHelper
    {
        public const string PF_Description = "description.md", PF_Input = "input.md", PF_Output = "output.md", PF_Hint = "hint.md";

        public static ProblemDescription LoadFromPlainText(string root)
        {
            string description = Path.Combine(root, PF_Description);
            string input = Path.Combine(root, PF_Input);
            string output = Path.Combine(root, PF_Output);
            string hint = Path.Combine(root, PF_Hint);
            return new ProblemDescription
            {
                Description = TextIO.ReadAllInUTF8(description),
                Input = TextIO.ReadAllInUTF8(input),
                Output = TextIO.ReadAllInUTF8(output),
                Hint = TextIO.ReadAllInUTF8(hint)
            };
        }

        public static void Initialize(string root)
        {
            string description = Path.Combine(root, PF_Description);
            string input = Path.Combine(root, PF_Input);
            string output = Path.Combine(root, PF_Output);
            string hint = Path.Combine(root, PF_Hint);

            TextIO.WriteAllInUTF8(description, string.Empty);
            TextIO.WriteAllInUTF8(input, string.Empty);
            TextIO.WriteAllInUTF8(output, string.Empty);
            TextIO.WriteAllInUTF8(hint, string.Empty);
        }

        public static void Extract(string root, ProblemDescription value)
        {
            string description = Path.Combine(root, PF_Description);
            string input = Path.Combine(root, PF_Input);
            string output = Path.Combine(root, PF_Output);
            string hint = Path.Combine(root, PF_Hint);

            TextIO.WriteAllInUTF8(description, value.Description);
            TextIO.WriteAllInUTF8(input, value.Input);
            TextIO.WriteAllInUTF8(output, value.Output);
            TextIO.WriteAllInUTF8(hint, value.Hint);
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

        public string Sample { get; private set; }

        public string Test { get; private set; }

        public ProblemDescription GetDescription() => ProblemDescriptionHelper.LoadFromPlainText(Description);

        public ProblemMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.Id = Path.GetFileName(Root);
            return res;
        }

        public ITestCaseProvider GetSample(string id)
        {
            string path = Path.Combine(Sample, id);
            return Directory.Exists(path) ? new TestCaseProvider(path) : null;
        }

        public IEnumerable<ITestCaseProvider> GetSamples() => Directory.GetDirectories(Sample).Select(path => new TestCaseProvider(path));

        public ITestCaseProvider GetTest(string id)
        {
            string path = Path.Combine(Test, id);
            return Directory.Exists(path) ? new TestCaseProvider(path) : null;
        }

        public IEnumerable<ITestCaseProvider> GetTests() => Directory.GetDirectories(Test).Select(path => new TestCaseProvider(path));

        public void SetMetadata(ProblemMetadata value) => TextIO.WriteAllInUTF8(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented));

        public ProblemProvider(string root)
        {
            Root = root;
            Id = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Description = Path.Combine(Root, PD_Description);
            Sample = Path.Combine(Root, PD_Sample);
            Test = Path.Combine(Root, PD_Test);
        }

        public static ProblemProvider Initialize(string root, ProblemMetadata metadata = null, ProblemDescription description = null)
        {
            var res = new ProblemProvider(root);
            if (metadata == null) metadata = new ProblemMetadata { Author = "", Name = "Untitled", Source = "" };
            metadata.Id = res.Id;

            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
            Directory.CreateDirectory(res.Description);
            if (description == null)
                ProblemDescriptionHelper.Initialize(res.Description);
            else
                ProblemDescriptionHelper.Extract(res.Description, description);

            Directory.CreateDirectory(res.Sample);
            string spath = Path.Combine(res.Sample, "0");
            Directory.CreateDirectory(spath);
            TestCaseProvider.Initialize(spath);

            Directory.CreateDirectory(res.Test);
            string tpath = Path.Combine(res.Test, "0");
            Directory.CreateDirectory(tpath);
            TestCaseProvider.Initialize(tpath);

            return res;
        }
    }
}
