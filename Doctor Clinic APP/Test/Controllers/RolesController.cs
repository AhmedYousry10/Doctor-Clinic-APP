using Doctor_CLinic_API.IServices;
using Doctor_CLinic_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Doctor_CLinic_API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("getRoles")]
        [SwaggerOperation(summary: "Get all roles", description: "Get all roles")]
        [SwaggerResponse(statusCode: 200, description: "Success")]
        public async Task<ActionResult> GetRoles()
        {
            var rolesList = await _roleService.GetRoleAsync();
            return Ok(rolesList);
        }

        [HttpGet("getUserRoles")]
        [SwaggerOperation(summary: "Get roles for a user by their email", description: "Get roles for a user by their email")]
        [SwaggerResponse(statusCode: 200, description: "Success")]
        public async Task<ActionResult> GetUserRoles(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email is required.");
            }
            var userData = await _roleService.GetUserRoleAsync(userEmail);
            return Ok(userData);
        }

        [HttpPost("addRoles")]
        [SwaggerOperation(summary: "Add new roles if they don't already exist", description: "Add new roles if they don't already exist")]
        [SwaggerResponse(statusCode: 200, description: "Success")]
        public async Task<ActionResult> AddRoles(string[] roles)
        {
            if (roles == null || roles.Length == 0)
            {
                return BadRequest("At least one role must be provided.");
            }

            var newRoles = await _roleService.AddRolesAsync(roles);
            if(newRoles.Count == 0)
            {
                return NoContent();
            }
            return Ok($"New role(s) added: {string.Join(", ", newRoles)}");
        }

        [HttpPost("addUserRoles")]
        [SwaggerOperation(summary: "Add roles to a user", description: "Add roles to a user")]
        [SwaggerResponse(statusCode: 201, description: "Success")]
        public async Task<ActionResult> AddUserRoles([FromBody] UserRole userRole)
        {
            if (userRole == null || string.IsNullOrWhiteSpace(userRole.UserEmail) || userRole.Roles == null)
            {
                return BadRequest("User email and roles must be provided.");
            }

            var result = await _roleService.AddUerRoleAsync(userRole.UserEmail, userRole.Roles);
            if (!result)
            {
                return BadRequest();
            }
            return StatusCode((int)HttpStatusCode.Created, result);

        }

        [HttpDelete("removeUserRoles")]
        [SwaggerOperation(summary: "Remove roles from a user", description: "Remove roles from a user")]
        [SwaggerResponse(statusCode: 200, description: "Roles removed successfully.")]
        [SwaggerResponse(statusCode: 400, description: "Invalid input provided.")]
        [SwaggerResponse(statusCode: 404, description: "User or roles not found.")]
        public async Task<ActionResult> RemoveUserRoles([FromBody] UserRole userRole)
        {
            if (userRole == null ||
                string.IsNullOrWhiteSpace(userRole.UserEmail) ||
                userRole.Roles == null ||
                !userRole.Roles.Any())
            {
                return BadRequest("User email and roles must be provided.");
            }

            try
            {
                var result = await _roleService.RemoveUserRoleAsync(userRole.UserEmail, userRole.Roles);
                if (!result)
                {
                    return NotFound("User or roles not found.");
                }

                return Ok(new { message = "Roles removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
