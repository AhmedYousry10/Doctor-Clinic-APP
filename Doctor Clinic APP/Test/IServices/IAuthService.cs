using System.Threading.Tasks;
using Doctor_CLinic_API.DTO;

namespace Doctor_CLinic_API.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO);
        Task EnsureDefaultAdminAsync();
    }
}
