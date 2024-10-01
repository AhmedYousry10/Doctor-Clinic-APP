
using Doctor_CLinic_API.DTO;

namespace Doctor_CLinic_API.IServices
{
    public interface IPatientService
    {

        Task<PatientDTO> GetPatientByIdAsync(int id);
        Task<List<PatientDTO>> GetPatientsAsync(int pageNumber, int pageSize);
        Task AddPatientAsync(PatientDTO patientDto, int id);
        Task UpdatePatientAsync(int id, PatientDTO patientDto, int userId);
        Task DeletePatientAsync(int id, int userId);
        Task<List<PatientDTO>> GetPatientsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
