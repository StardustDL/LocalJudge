using System;
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
        static readonly object lock_admin = new object();

        [HttpGet]
        public ActionResult<string> GetRootDirectory()
        {
            return Program.Workspace.Root;
        }

        [HttpPut("init")]
        public void Initialize()
        {
            lock (lock_admin)
            {
                Workspace.Initialize(Program.Workspace.Root);
            }
        }

        [HttpPut("seed")]
        public void SeedData()
        {
            lock (lock_admin)
            {
                string workspace = Program.Workspace.Root;
                var ws = Workspace.Initialize(workspace);
                ws.Problems.Create("0");
            }
        }
    }
}