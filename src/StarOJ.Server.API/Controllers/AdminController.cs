using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StarOJ.Core;
using StarOJ.Core.Problems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StarOJ.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        static readonly object lock_admin = new object();
        private readonly IWorkspace _workspace;

        public AdminController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        [HttpPut("init")]
        public void Initialize()
        {
            lock (lock_admin)
            {
                _workspace.Initialize();
            }
        }
    }
}