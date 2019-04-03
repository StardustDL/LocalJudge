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
        public ActionResult<IEnumerable<RoleMetadata>> GetAll()
        {
            return Ok(_workspace.Roles.GetAll().Select(item => item.GetMetadata()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<RoleMetadata> Get(string id)
        {
            var res = _workspace.Roles.Get(id)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<RoleMetadata> GetByName(string name)
        {
            var res = _workspace.Roles.GetByName(name)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _workspace.Roles.Delete(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<RoleMetadata> Create([FromBody] RoleMetadata data)
        {
            if (string.IsNullOrEmpty(data.Id)) data.Id = Guid.NewGuid().ToString();

            try
            {
                var res = _workspace.Roles.Create(data);
                return Created($"users/{res.Id}", res.GetMetadata());
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
        public ActionResult Update([FromBody] RoleMetadata data)
        {
            try
            {
                var prov = _workspace.Roles.Get(data.Id);
                if (prov != null)
                {
                    prov.SetMetadata(data);
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