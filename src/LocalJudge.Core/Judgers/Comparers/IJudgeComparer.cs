using System.Collections.Generic;
using System.IO;

namespace LocalJudge.Core.Judgers.Comparers
{
    public interface IJudgeComparer
    {
        IEnumerable<Issue> Compare(TextReader expected, TextReader real);
    }
}
