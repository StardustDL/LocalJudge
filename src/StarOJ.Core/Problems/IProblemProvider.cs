using StarOJ.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StarOJ.Core.Problems
{
    public interface IProblemProvider : IHasId<string>, IHasMetadata<ProblemMetadata>
    {
        ProblemDescription GetDescription();

        IEnumerable<ITestCaseProvider> GetSamples();

        ITestCaseProvider GetSample(string id);

        IEnumerable<ITestCaseProvider> GetTests();

        ITestCaseProvider GetTest(string id);
    }
}
