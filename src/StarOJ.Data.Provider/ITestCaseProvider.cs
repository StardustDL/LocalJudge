using System.IO;
using System.Threading.Tasks;

namespace StarOJ.Core.Problems
{
    public interface ITestCaseProvider : IHasId<string>, IHasMetadata<TestCaseMetadata>
    {
        Task<DataPreview> GetInputPreview(int maxbytes);

        Task<DataPreview> GetOutputPreview(int maxbytes);

        Task<Stream> GetInput();

        Task<Stream> GetOutput();

        Task SetInput(Stream value);

        Task SetOutput(Stream value);
    }
}
