using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalJudge.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalJudge.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            return Ok(Program.Workspace.Users.GetAll().Select(item => item.GetMetadata()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<User> Get(string id)
        {
            var res = Program.Workspace.Users.Get(id)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<User> GetByName(string name)
        {
            var res = Program.Workspace.Users.GetByName(name)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            Program.Workspace.Users.Delete(id);
        }

        /*[HttpPut("{id}/roles/{roleName}")]
        public void AddRole(string id, string roleName)
        {
            var user = Program.Workspace.Users.Get(id)?.GetMetadata();
            if (user == null) return;
            var role = Program.Workspace.Roles.GetByName(roleName);
            if (role == null) return;
            var index = user.Roles.FindIndex(x => x.Id == role.Id);
            if (index != -1) return;
            user.Roles.Add(role.GetMetadata());
            Program.Workspace.Users.Update(user);
        }

        [HttpDelete("{id}/roles/{roleName}")]
        public void DeleteRole(string id, string roleName)
        {
            var user = Program.Workspace.Users.Get(id)?.GetMetadata();
            if (user == null) return;
            var role = Program.Workspace.Roles.GetByName(roleName);
            if (role == null) return;
            var index = user.Roles.FindIndex(x => x.Id == role.Id);
            if (index == -1) return;
            user.Roles.RemoveAt(index);
            Program.Workspace.Users.Update(user);
        }*/

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<User> Create([FromBody] User data)
        {
            if (string.IsNullOrEmpty(data.Id)) data.Id = Guid.NewGuid().ToString();

            try
            {
                var res = Program.Workspace.Users.Create(data.Id, data);
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
        public ActionResult Update([FromBody] User data)
        {
            try
            {
                if (Program.Workspace.Users.Update(data))
                {
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