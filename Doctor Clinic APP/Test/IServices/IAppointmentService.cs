using Doctor_CLinic_API.DTO;
using System.Threading.Tasks;

namespace Doctor_CLinic_API.IServices
{
    public interface IAppointmentService
    {
        Task<AppointmentDTO> GetAppointmentByIdAsync(int id);
        Task<List<AppointmentDTO>> GetAppointmentsAsync(int pageNumber, int pageSize);
        Task AddAppointmentAsync(AppointmentDTO appointmentDto, int userId);
        Task UpdateAppointmentAsync(int id, AppointmentDTO appointmentDto);
        Task CancelAppointmentAsync(int id);
        Task<List<AppointmentDTO>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<AppointmentDTO>> GetAppointmentsByUserIdAsync(int userId);
    }
}
