using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LocalJudge.Core;
using LocalJudge.Core.Problems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalJudge.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> GetRootDirectory()
        {
            return Program.Workspace;
        }

        [HttpPut("init")]
        public void Initialize()
        {
            string workspace = Program.Workspace;
            Workspace.Initialize(workspace);
        }

        [HttpPut("seed")]
        public void SeedData()
        {
            string workspace = Program.Workspace;
            var ws = Workspace.Initialize(workspace);
            ws.Problems.Create("0");
        }
    }
}