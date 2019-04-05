using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StarOJ.Core.Submissions
{
    public interface ISubmissionListProvider : IItemListProvider<ISubmissionProvider,SubmissionMetadata>
    {
    }
}
