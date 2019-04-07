using StarOJ.Core.Identity;
using StarOJ.Core.Problems;
using StarOJ.Core.Submissions;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider
{
    public interface IWorkspace
    {

        Task<WorkspaceConfig> GetConfig();

        IProblemListProvider Problems { get; }

        ISubmissionListProvider Submissions { get; }

        IUserListProvider Users { get; }

        IRoleListProvider Roles { get; }

        IStatisticProvider Statistics { get; }

        bool HasInitialized { get; }

        Task Initialize();

        Task Clear();
    }
}
