using LocalJudge.Core.Problems;
using System.IO;

namespace LocalJudge.Core
{
    public class Workspace
    {
        public const string PD_Problem = "problems";

        public string Root { get; private set; }

        public ProblemManager Problems { get; private set; }

        public Workspace(string root)
        {
            Root = root;
            Problems = new ProblemManager(Path.Combine(Root, PD_Problem));
        }

        public static Workspace Initialize(string root)
        {
            var res = new Workspace(root);
            Directory.CreateDirectory(res.Problems.Root);
            return res;
        }
    }
}
