using System.IO;
using System.Threading.Tasks;

namespace StarOJ.Core.Submissions
{
    public interface ISubmissionProvider : IHasId<string>, IHasMetadata<SubmissionMetadata>
    {
        Task<SubmissionResult> GetResult();

        Task SetResult(SubmissionResult value);

        Task<Stream> GetCode();

        Task SetCode(Stream value);
    }
}
