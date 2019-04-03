using System.Collections.Generic;
using StarOJ.Core;
using StarOJ.Core.Judgers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StarOJ.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        public WorkspaceController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        [HttpGet("lang")]
        public async Task<ActionResult<IEnumerable<ProgrammingLanguage>>> GetSupportLanguages()
        {
            return Ok((await _workspace.GetConfig()).GetSupportLanguages());
        }

        [HttpGet("hasinit")]
        public ActionResult<bool> GetHasInitialized()
        {
            return Ok(_workspace.HasInitialized);
        }
    }
}