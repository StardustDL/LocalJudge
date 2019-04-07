using Microsoft.AspNetCore.Mvc;
using StarOJ.Core.Identity;
using StarOJ.Core.Problems;
using StarOJ.Data.Provider;
using System.Threading.Tasks;

namespace StarOJ.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        public StatisticsController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        [HttpGet("user/{id}")]
        public async Task<UserStatistics> GetUser(string id)
        {
            return await _workspace.Statistics.GetUser(id);
        }

        [HttpGet("problem/{id}")]
        public async Task<ProblemStatistics> GetProblem(string id)
        {
            return await _workspace.Statistics.GetProblem(id);
        }
    }
}