using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LocalJudge.Core.Submissions
{
    public interface ISubmissionListProvider : IItemListProvider<ISubmissionProvider,SubmissionMetadata>
    {
    }
}
