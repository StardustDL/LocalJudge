using System;
using System.Collections.Generic;
using System.Linq;
using StarOJ.Core;
using StarOJ.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StarOJ.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        public UsersController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserMetadata>> GetAll()
        {
            return Ok(_workspace.Users.GetAll().Select(item => item.GetMetadata()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<UserMetadata> Get(string id)
        {
            var res = _workspace.Users.Get(id)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<UserMetadata> GetByName(string name)
        {
            var res = _workspace.Users.GetByName(name)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _workspace.Users.Delete(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<UserMetadata> Create([FromBody] UserMetadata data)
        {
            if (string.IsNullOrEmpty(data.Id)) data.Id = Guid.NewGuid().ToString();

            try
            {
                var res = _workspace.Users.Create(data);
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
        public ActionResult Update([FromBody] UserMetadata data)
        {
            try
            {
                var prov = _workspace.Users.Get(data.Id);
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