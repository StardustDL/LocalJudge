using LocalJudge.Core.Helpers;
using LocalJudge.Core.Identity;
using LocalJudge.Core.Judgers;
using LocalJudge.Core.Problems;
using LocalJudge.Core.Submissions;
using System;
using System.IO;

namespace LocalJudge.Core
{
    public interface IWorkspace
    {

        WorkspaceConfig GetConfig();

        IProblemListProvider Problems { get; }

        ISubmissionListProvider Submissions { get; }

        IUserListProvider Users { get; }

        IRoleListProvider Roles { get;}

        bool HasInitialized{get;}

        void Initialize();
    }
}
