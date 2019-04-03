using LocalJudge.Core.Helpers;
using LocalJudge.Core.Judgers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Submissions
{
    public interface ISubmissionProvider : IHasId<string>,IHasMetadata<SubmissionMetadata>
    {
        SubmissionResult GetResult();

        void SetResult(SubmissionResult value);

        string GetCode();

        void SetCode(string value);
    }
}
