using StarOJ.Core.Helpers;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using StarOJ.Core.Problems;
using StarOJ.Core.Submissions;
using System;
using System.IO;

namespace StarOJ.Core
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
