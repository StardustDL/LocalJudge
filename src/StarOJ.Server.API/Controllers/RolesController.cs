using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarOJ.Core.Identity;
using StarOJ.Data.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarOJ.Server.API.Controllers
{
    [Route("[controller]")]
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
            IEnumerable<IRoleProvider> all = await _workspace.Roles.GetAll();
            List<RoleMetadata> ans = new List<RoleMetadata>();
            foreach (IRoleProvider v in all)
            {
                ans.Add(await v.GetMetadata());
            }
            return Ok(ans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleMetadata>> Get(string id)
        {
            IRoleProvider res = await _workspace.Roles.Get(id);
            if (res != null)
                return Ok(await res.GetMetadata());
            else
                return NotFound();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<RoleMetadata>> GetByName(string name)
        {
            IRoleProvider res = await _workspace.Roles.GetByName(name);
            if (res != null)
                return Ok(await res.GetMetadata());
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public Task Delete(string id)
        {
            return _workspace.Roles.Delete(id);
        }

        [HttpPost]
        public async Task<ActionResult<RoleMetadata>> Create([FromBody] RoleMetadata data)
        {
            try
            {
                IRoleProvider res = await _workspace.Roles.Create(data);
                return Created($"users/{res.Id}", await res.GetMetadata());
            }
            catch
            {
                return Forbid();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Update([FromBody] RoleMetadata data)
        {
            try
            {
                IRoleProvider prov = await _workspace.Roles.Get(data.Id);
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