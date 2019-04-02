using System.Collections.Generic;
using LocalJudge.Core.Judgers;
using Microsoft.AspNetCore.Mvc;

namespace LocalJudge.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {

        [HttpGet("lang")]
        public ActionResult<IEnumerable<ProgrammingLanguage>> GetSupportLanguages()
        {
            return Ok(Program.Workspace.GetConfig().GetSupportLanguages());
        }

        [HttpGet("hasinit")]
        public ActionResult<bool> GetHasInitialized()
        {
            return Ok(Program.Workspace.HasInitialized);
        }
    }
}