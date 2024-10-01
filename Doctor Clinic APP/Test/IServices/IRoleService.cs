using Doctor_CLinic_API.Models;

namespace Doctor_CLinic_API.IServices
{
    public interface IRoleService
    {
        Task<List<Role>> GetRoleAsync();
        Task<List<string>> GetUserRoleAsync(string emailId);
        Task<List<string>> AddRolesAsync(string[] roles);
        Task<bool> AddUerRoleAsync(string userEmail, string[] roles);
        Task<bool> RemoveUserRoleAsync(string userEmail, string[] roles);
        Task EnsureDefaultRolesAsync();
    }
}
