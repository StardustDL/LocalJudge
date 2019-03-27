using LocalJudge.Core.Helpers;
using System.IO;

namespace LocalJudge.Core.Problems
{
    public class ProblemDescription
    {
        public const string PF_Description = "description.md", PF_Input = "input.md", PF_Output = "output.md", PF_Hint = "hint.md";

        public string Description { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }

        public string Hint { get; set; }

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

        public void Extract(string root)
        {
            string description = Path.Combine(root, PF_Description);
            string input = Path.Combine(root, PF_Input);
            string output = Path.Combine(root, PF_Output);
            string hint = Path.Combine(root, PF_Hint);

            TextIO.WriteAllInUTF8(description, Description);
            TextIO.WriteAllInUTF8(input, Input);
            TextIO.WriteAllInUTF8(output, Output);
            TextIO.WriteAllInUTF8(hint, Hint);
        }
    }
}
