using System;
using System.Collections.Generic;
using System.Linq;
using StarOJ.Core;
using StarOJ.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace StarOJ.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        public UsersController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserMetadata>>> GetAll()
        {
            var all = await _workspace.Users.GetAll();
            List<UserMetadata> ans = new List<UserMetadata>();
            foreach (var v in all)
            {
                ans.Add(await v.GetMetadata());
            }
            return Ok(ans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserMetadata>> Get(string id)
        {
            var res = await _workspace.Users.Get(id);
            if (res != null)
                return Ok(await res.GetMetadata());
            else
                return NotFound();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<UserMetadata>> GetByName(string name)
        {
            var res = await _workspace.Users.GetByName(name);
            if (res != null)
                return Ok(await res.GetMetadata());
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _workspace.Users.Delete(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<UserMetadata>> Create([FromBody] UserMetadata data)
        {
            try
            {
                var res = await _workspace.Users.Create(data);
                return Created($"users/{res.Id}", await res.GetMetadata());
            }
            catch
            {
                return Forbid();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Update([FromBody] UserMetadata data)
        {
            try
            {
                var prov = await _workspace.Users.Get(data.Id);
                if (prov != null)
                {
                    await prov.SetMetadata(data);
                    return Accepted();
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return Forbid();
            }
        }
    }
}