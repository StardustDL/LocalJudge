﻿using Microsoft.AspNetCore.Mvc;
using StarOJ.Core.Judgers;
using StarOJ.Data.Provider;
using System.Collections.Generic;
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

        [HttpPut("init")]
        public async Task Initialize()
        {
            await _workspace.Initialize();
        }

        [HttpPut("clear")]
        public async Task Clear()
        {
            await _workspace.Clear();
        }

        [HttpGet("lang")]
        public async Task<ActionResult<IEnumerable<ProgrammingLanguage>>> GetSupportLanguages()
        {
            return Ok((await _workspace.GetConfig()).SupportLanguages);
        }

        [HttpGet("hasinit")]
        public ActionResult<bool> GetHasInitialized()
        {
            return Ok(_workspace.HasInitialized);
        }
    }
}