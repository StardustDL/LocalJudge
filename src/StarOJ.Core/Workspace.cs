using StarOJ.Core.Helpers;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using StarOJ.Core.Problems;
using StarOJ.Core.Submissions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StarOJ.Core
{
    public interface IWorkspace
    {

        Task<WorkspaceConfig> GetConfig();

        IProblemListProvider Problems { get; }

        ISubmissionListProvider Submissions { get; }

        IUserListProvider Users { get; }

        IRoleListProvider Roles { get;}

        bool HasInitialized{get;}

        Task Initialize();
    }
}
