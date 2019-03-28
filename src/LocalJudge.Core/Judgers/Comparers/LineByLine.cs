using LocalJudge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Judgers.Comparers
{
    public class LineByLine : IJudgeComparer
    {
        public IEnumerable<Issue> Compare(TextReader expected, TextReader real)
        {
            List<string> lexpected = TextIO.SplitLines(expected).Select(x => x.TrimEnd('\r', '\n')).ToList(), lreal = TextIO.SplitLines(real).Select(x => x.TrimEnd('\r', '\n')).ToList();
            while (lexpected.Count > 0 && string.IsNullOrEmpty(lexpected.Last())) lexpected.RemoveAt(lexpected.Count - 1);
            while (lreal.Count > 0 && string.IsNullOrEmpty(lreal.Last())) lreal.RemoveAt(lreal.Count - 1);

            if (lexpected.Count != lreal.Count)
            {
                yield return new Issue(IssueLevel.Error,$"The count of lines are not equal: expected {expected}, but real {real}.");
                yield break;
            }
            for (int i = 0; i < lexpected.Count; i++)
            {
                if (lexpected[i].TrimEnd() != lreal[i].TrimEnd())
                {
                    yield return new Issue(IssueLevel.Error, $"Contents at line {i + 1} are not equal.");
                }
            }
        }
    }
}
