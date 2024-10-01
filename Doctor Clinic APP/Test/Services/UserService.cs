using AutoMapper;
using Doctor_CLinic_API.Data;
using Doctor_CLinic_API.DTO;
using Doctor_CLinic_API.IServices;
using Doctor_CLinic_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Doctor_CLinic_API.Services
{
    public class UserService : IUserService
    {
        private readonly appContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserService(appContext db, IMapper mapper, UserManager<User> userManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
        }

        // Create a new user
        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Password))
            {
                throw new ArgumentException("Email and Password are required.");
            }

            var user = _mapper.Map<User>(userDto);
            user.UserName = userDto.Email;

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return _mapper.Map<UserDTO>(user);
        }

        // Remove a user by their email
        public async Task<string> DeleteUserAsync(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new ArgumentException("User email cannot be null or empty.");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return "User deleted successfully.";
            }
            else
            {
                throw new Exception("Failed to delete user.");
            }
        }

        // Get all users
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null ? _mapper.Map<UserDTO>(user) : null;
        }

        // Update a user
        public async Task<UserDTO> UpdateUserAsync(UserDTO userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
            if (user == null) return null;

            user.Name = userDto.Name;
            user.Email = userDto.Email;

            // Only update password if it's provided
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userDto.Password);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return _mapper.Map<UserDTO>(user);
        }
    }
}
