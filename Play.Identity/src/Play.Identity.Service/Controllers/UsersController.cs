using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Play.Identity.Service.Dtos;
using Play.Identity.Service.Entities;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Play.Identity.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = LocalApi.PolicyName, Roles = Roles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        public UsersController(UserManager<ApplicationUser> _userManager)
        {
            userManager = _userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAsync()
        {
            try
            {
                var users = userManager.Users.ToList().Select(user => user.AsDto());
                return Ok(users);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user is null)
                {
                    return NotFound();
                }
                return Ok(user.AsDto());
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateUserDto userDto)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user is null)
                {
                    return NotFound();
                }
                user.Email = userDto.Email;
                user.UserName = userDto.Email; // username is email
                user.Gil = userDto.Gil;
                await userManager.UpdateAsync(user);
                return NoContent();
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user is null)
                {
                    return NotFound();
                }
                await userManager.DeleteAsync(user);
                return NoContent();
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}