using Doctor_CLinic_API.IServices;
using Doctor_CLinic_API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Doctor_CLinic_API.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager) 
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Get all roles
        public async Task<List<Role>> GetRoleAsync()
        {
            var roleList = _roleManager.Roles.ToList(); 
            return roleList;
        }

        // Get roles for a user by their email
        public async Task<List<string>> GetUserRoleAsync(string emailId)
        {
            var user = await _userManager.FindByEmailAsync(emailId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            return userRoles.ToList();
        }

        // Add new roles if they don't already exist
        public async Task<List<string>> AddRolesAsync(string[] roles)
        {
            var roleList = new List<string>();
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new Role { Name = role });
                    if (result.Succeeded)
                    {
                        roleList.Add(role);
                    }
                    else
                    {
                        throw new Exception($"Failed to create role: {role}");
                    }
                }
            }
            return roleList;
        }

        // Assign roles to a user by their email
        public async Task<bool> AddUerRoleAsync(string userEmail, string[] roles)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                throw new Exception("User not found.");

            }

            var validRoles = await ExistsRolesAsync(roles);
            if (validRoles.Count != roles.Length)
            {
                throw new Exception("Some roles do not exist.");
            }

            var result = await _userManager.AddToRolesAsync(user, validRoles);
            return result.Succeeded;
        }

        // Helper method to check if roles exist
        private async Task<List<string>> ExistsRolesAsync(string[] roles)
        {
            var roleList = new List<string>();
            foreach(var role in roles)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (roleExist)
                {
                    roleList.Add(role);
                }
            }
            return roleList;
        }

        // Remove roles from a user by their email
        public async Task<bool> RemoveUserRoleAsync(string userEmail, string[] roles)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            return result.Succeeded;
        }

        // Ensure default roles exist
        public async Task EnsureDefaultRolesAsync()
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new Role { Name = role });
                }
            }
        }


    }
}
