using StarOJ.Core.Identity;
using StarOJ.Core.Problems;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider
{
    public interface IStatisticProvider
    {
        Task<UserStatistics> GetUser(string id);

        Task<ProblemStatistics> GetProblem(string id);
    }
}
