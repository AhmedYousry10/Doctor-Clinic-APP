using AutoMapper;
using Doctor_CLinic_API.Data;
using Doctor_CLinic_API.DTO;
using Doctor_CLinic_API.IServices;
using Doctor_CLinic_API.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Doctor_CLinic_API.Services
{
    public class PatientService : IPatientService
    {
        private readonly appContext _db;
        private readonly IMapper _mapper;

        public PatientService(appContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // Get Patient by id
        public async Task<PatientDTO> GetPatientByIdAsync(int id)
        {
            var patient = await _db.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                throw new KeyNotFoundException("Patient not found.");
            }

            return _mapper.Map<PatientDTO>(patient);
        }

        // Get all patients
        public async Task<List<PatientDTO>> GetPatientsAsync(int pageNumber, int pageSize)
        {
            var patients = await _db.Patients
                .Include(p => p.Appointments)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<PatientDTO>>(patients);
        }

        // Add a patient
        public async Task AddPatientAsync(PatientDTO patientDto, int userId)
        {
            if (patientDto == null)
            {
                throw new ArgumentNullException(nameof(patientDto), "Patient data cannot be null.");
            }

            var validationResults = ValidatePatientDto(patientDto);
            if (validationResults.Any())
            {
                throw new ValidationException(string.Join(", ", validationResults));
            }

            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ArgumentException("Invalid user ID. User does not exist.");
            }

            var patient = _mapper.Map<Patient>(patientDto);
            patient.UserId = userId;

            await _db.Patients.AddAsync(patient);
            await _db.SaveChangesAsync();
        }

        // Update a patient
        public async Task UpdatePatientAsync(int id, PatientDTO patientDto, int userId)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new KeyNotFoundException("Patient not found.");
            }

            var validationResults = ValidatePatientDto(patientDto);
            if (validationResults.Any())
            {
                throw new ValidationException(string.Join(", ", validationResults));
            }

            _mapper.Map(patientDto, patient);
            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
        }

        // Delete a patient
        public async Task DeletePatientAsync(int id, int userId)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null)
            {
                throw new KeyNotFoundException("Patient not found.");
            }

            _db.Patients.Remove(patient);
            await _db.SaveChangesAsync();
        }

        // Get patients by date range
        public async Task<List<PatientDTO>> GetPatientsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var patients = await _db.Patients
                .Include(p => p.Appointments)
                .Where(p => p.Appointments.Any(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate))
                .ToListAsync();

            return _mapper.Map<List<PatientDTO>>(patients);
        }

        // Helper method for validation
        private IEnumerable<string> ValidatePatientDto(PatientDTO patientDto)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(patientDto);
            if (!Validator.TryValidateObject(patientDto, context, results, true))
            {
                return results.Select(r => r.ErrorMessage);
            }
            return Enumerable.Empty<string>();
        }
    }
}
