using StarOJ.Core.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StarOJ.Core.Problems
{
    public interface ITestCaseProvider : IHasId<string>, IHasMetadata<TestCaseMetadata>
    {
        Task<DataPreview> GetInputPreview(int maxbytes);

        Task<DataPreview> GetOutputPreview(int maxbytes);

        Task<string> GetInput();

        Task<string> GetOutput();

        Task SetInput(string value);

        Task SetOutput(string value);
    }
}
