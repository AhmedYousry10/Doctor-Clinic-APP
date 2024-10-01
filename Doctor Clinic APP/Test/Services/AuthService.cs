using System.Linq;
using System.Threading.Tasks;
using Doctor_CLinic_API.DTO;
using Doctor_CLinic_API.Models;
using Doctor_CLinic_API.IServices;
using Microsoft.AspNetCore.Identity;
using Doctor_CLinic_API.Enums;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Net.Mail;

namespace Doctor_CLinic_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenService _jwtTokenService;

        public AuthService(UserManager<User> userManager, JwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            // Validate email format
            if (!IsValidEmail(registerDTO.Email))
            {
                throw new Exception("Invalid email format.");
            }

            // Check if user already exists
            if (await _userManager.FindByEmailAsync(registerDTO.Email) != null)
            {
                throw new Exception("Email is already registered.");
            }
            var user = new User
            {
                UserName = registerDTO.Email,
                Name = registerDTO.Name,
                Email = registerDTO.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            // Check if password meets security requirements
            if (!result.Succeeded)
            {
                throw new Exception("User registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Assign the default role "User"
            if (!await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            var userRole = await _userManager.GetRolesAsync(user);
            var token = await _jwtTokenService.GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Message = "User registered successfully.",
                userId = user.Id,
                Role = userRole.ToArray()
            };
        }


        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            // Validate email format
            if (!IsValidEmail(loginDTO.Email))
            {
                throw new Exception("Invalid email format.");
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the user is locked out (optional, if you have this feature)
            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new Exception("User account is locked out.");
            }

            // Validate password
            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result)
            {
                throw new Exception("Invalid login attempt.");
            }
            var userRole = await _userManager.GetRolesAsync(user);
            var token = await _jwtTokenService.GenerateJwtToken(user);
            return new AuthResponseDTO
            {
                Message = "Login successfully.",
                userId = user.Id,
                Token = token,
                Role = userRole.ToArray(),
            };
        }


        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task EnsureDefaultAdminAsync()
        {
            var adminEmail = "admin@admin.com";
            var adminPassword = "Admin@12345";
            var adminName = "Admin";

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = adminName
                };

                var result = await _userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

    }
}
