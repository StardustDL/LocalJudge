using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarOJ.Core;
using StarOJ.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StarOJ.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        public RolesController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleMetadata>>> GetAll()
        {
            var all = await _workspace.Roles.GetAll();
            List<RoleMetadata> ans = new List<RoleMetadata>();
            foreach (var v in all)
            {
                ans.Add(await v.GetMetadata());
            }
            return Ok(ans);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<RoleMetadata>> Get(string id)
        {
            var res = await (await _workspace.Roles.Get(id))?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<RoleMetadata>> GetByName(string name)
        {
            var res = await (await _workspace.Roles.GetByName(name))?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _workspace.Roles.Delete(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<RoleMetadata>> Create([FromBody] RoleMetadata data)
        {
            try
            {
                var res = await _workspace.Roles.Create(data);
                return Created($"users/{res.Id}", await res.GetMetadata());
            }
            catch
            {
                return Forbid();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Update([FromBody] RoleMetadata data)
        {
            try
            {
                var prov = await _workspace.Roles.Get(data.Id);
                if (prov != null)
                {
                    await prov.SetMetadata(data);
                    return Ok();
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