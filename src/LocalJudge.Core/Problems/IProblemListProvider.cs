using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Problems
{
    public interface IProblemListProvider : IItemListProvider<IProblemProvider,ProblemMetadata>
    {
    }
}
