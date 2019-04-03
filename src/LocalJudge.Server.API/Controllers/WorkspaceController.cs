using System.Collections.Generic;
using LocalJudge.Core;
using LocalJudge.Core.Judgers;
using Microsoft.AspNetCore.Mvc;

namespace LocalJudge.Server.API.Controllers
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
        public ActionResult<IEnumerable<ProgrammingLanguage>> GetSupportLanguages()
        {
            return Ok(_workspace.GetConfig().GetSupportLanguages());
        }

        [HttpGet("hasinit")]
        public ActionResult<bool> GetHasInitialized()
        {
            return Ok(_workspace.HasInitialized);
        }
    }
}