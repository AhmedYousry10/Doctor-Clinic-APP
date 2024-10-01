using Doctor_CLinic_API.DTO;
using Doctor_CLinic_API.IServices;
using Doctor_CLinic_API.Models;
using Doctor_CLinic_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Doctor_CLinic_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("createUser")]
        [SwaggerOperation(summary: "Create a new user", description: "Create a new user")]
        [SwaggerResponse(201, "User created", typeof(UserDTO))]
        [SwaggerResponse(400, "Invalid user data")]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdUser = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("allUsers")]
        [SwaggerOperation(summary: "Get all users", description: "Get all users")]
        [SwaggerResponse(200, "Success", typeof(List<UserDTO>))]
        public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("{id}")]
        [SwaggerOperation(summary: "Get a user by ID", description: "Get a user by ID")]
        [SwaggerResponse(200, "Success", typeof(UserDTO))]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }


        [HttpPut("updateUser")]
        [SwaggerOperation(summary: "Update a user", description: "Update a user")]
        [SwaggerResponse(200, "Success", typeof(UserDTO))]
        [SwaggerResponse(400, "Invalid user data")]
        [SwaggerResponse(404, "User not found")]
        public async Task<ActionResult<UserDTO>> UpdateUser([FromBody] UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(userDto);
                if (updatedUser == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("deleteUser")]
        [SwaggerOperation(summary: "Delete a user", description: "Remove a user by their email address.")]
        [SwaggerResponse(statusCode: 200, description: "User removed successfully.")]
        [SwaggerResponse(statusCode: 400, description: "Bad request - User email is missing or invalid.")]
        [SwaggerResponse(statusCode: 404, description: "User not found.")]
        [SwaggerResponse(statusCode: 500, description: "Internal server error - An error occurred while processing your request.")]
        public async Task<ActionResult> DeleteUser([FromQuery] string userEmail)
        {
            // Validate the user email parameter
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest("User email is required.");
            }

            try
            {
                var resultMessage = await _userService.DeleteUserAsync(userEmail);
                return Ok(resultMessage);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
