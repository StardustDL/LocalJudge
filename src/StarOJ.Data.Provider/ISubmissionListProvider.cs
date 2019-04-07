using StarOJ.Core.Judgers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarOJ.Core.Submissions
{
    public interface ISubmissionListProvider : IItemListProvider<ISubmissionProvider, SubmissionMetadata>
    {
        Task<IEnumerable<ISubmissionProvider>> Query(string id, string problemId, string userId, ProgrammingLanguage? language, JudgeState? state);
    }
}
