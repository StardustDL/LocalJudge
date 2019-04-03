using LocalJudge.Core.Helpers;
using System;
using System.IO;

namespace LocalJudge.Core.Problems
{
    public interface ITestCaseProvider : IHasId<string>
    {
        TestCaseMetadata GetMetadata();

        DataPreview GetInputPreview(int maxbytes);

        DataPreview GetOutputPreview(int maxbytes);

        string GetInput();

        string GetOutput();
    }
}
