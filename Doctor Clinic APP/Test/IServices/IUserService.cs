using Doctor_CLinic_API.DTO;

namespace Doctor_CLinic_API.IServices
{
    public interface IUserService
    {
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task<UserDTO> UpdateUserAsync(UserDTO userDto);
        Task<string> DeleteUserAsync(string userEmail);
    }
}
