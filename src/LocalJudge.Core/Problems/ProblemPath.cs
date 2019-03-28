using LocalJudge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Problems
{
    public class ProblemPath
    {
        public const string PF_Profile = "profile.json";
        public const string PD_Description = "description", PD_Sample = "samples", PD_Test = "tests", PD_Extra = "extra";

        public string Root { get; private set; }

        public string ID { get; private set; }

        public string Profile { get; private set; }

        public ProblemMetadata GetMetadata()
        {
            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemMetadata>(TextIO.ReadAllInUTF8(Profile));
            res.ID = Path.GetFileName(Root);
            return res;
        }

        public string Description { get; private set; }

        public ProblemDescription GetDescription() => ProblemDescription.LoadFromPlainText(Description);

        public string Sample { get; private set; }

        public IEnumerable<TestCasePath> GetSamples() => Directory.GetDirectories(Sample).Select(path => new TestCasePath(path));

        public TestCasePath GetSample(string name)
        {
            string path = Path.Combine(Sample, name);
            return Directory.Exists(path) ? new TestCasePath(path) : null;
        }

        public string Test { get; private set; }

        public IEnumerable<TestCasePath> GetTests() => Directory.GetDirectories(Test).Select(path => new TestCasePath(path));

        public TestCasePath GetTest(string name)
        {
            string path = Path.Combine(Test, name);
            return Directory.Exists(path) ? new TestCasePath(path) : null;
        }

        public string Extra { get; private set; }

        public ProblemPath(string root)
        {
            Root = root;
            ID = Path.GetFileName(Root);
            Profile = Path.Combine(Root, PF_Profile);
            Description = Path.Combine(Root, PD_Description);
            Sample = Path.Combine(Root, PD_Sample);
            Test = Path.Combine(Root, PD_Test);
            Extra = Path.Combine(Root, PD_Extra);
        }

        public static ProblemPath Initialize(string root, ProblemMetadata metadata = null, ProblemDescription description = null)
        {
            var res = new ProblemPath(root);
            if (metadata == null) metadata = new ProblemMetadata { Author = "", Name = "Untitled", Source = "Origin" };
            metadata.ID = res.ID;

            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented));
            Directory.CreateDirectory(res.Description);
            if (description == null)
                ProblemDescription.Initialize(res.Description);
            else
                description.Extract(res.Description);

            Directory.CreateDirectory(res.Sample);
            string spath = Path.Combine(res.Sample, "0");
            Directory.CreateDirectory(spath);
            TestCasePath.Initialize(spath);

            Directory.CreateDirectory(res.Test);
            string tpath = Path.Combine(res.Test, "0");
            Directory.CreateDirectory(tpath);
            TestCasePath.Initialize(tpath);

            Directory.CreateDirectory(res.Extra);
            return res;
        }
    }
}
