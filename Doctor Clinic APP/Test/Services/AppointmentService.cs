using AutoMapper;
using Doctor_CLinic_API.Data;
using Doctor_CLinic_API.DTO;
using Doctor_CLinic_API.Enums;
using Doctor_CLinic_API.IServices;
using Doctor_CLinic_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor_CLinic_API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly appContext _db;
        private readonly IMapper _mapper;

        public AppointmentService(appContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }

            return _mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsAsync(int pageNumber, int pageSize)
        {
            var appointments = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.User)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

        public async Task AddAppointmentAsync(AppointmentDTO appointmentDto, [FromHeader] int userId)
        {
            // Check if the ModelState is valid (this is generally done automatically in controllers)
            if (appointmentDto == null)
            {
                throw new ArgumentNullException(nameof(appointmentDto), "Appointment data cannot be null.");
            }

            // Validate if the user exists in the database
            var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new KeyNotFoundException("User not found. Invalid user ID.");
            }

            // Validate if the patient exists in the database
            var patientExists = await _db.Patients.AnyAsync(p => p.Id == appointmentDto.PatientId);
            if (!patientExists)
            {
                throw new KeyNotFoundException("Patient not found. Invalid patient ID.");
            }

            // Map the AppointmentDTO to the Appointment entity
            var appointment = _mapper.Map<Appointment>(appointmentDto);

            // Set the valid userId (the doctor or user scheduling the appointment)
            appointment.UserId = userId;

            // Add the appointment to the database
            await _db.Appointments.AddAsync(appointment);

            // Save changes to the database
            await _db.SaveChangesAsync();
        }



        public async Task UpdateAppointmentAsync(int id, AppointmentDTO appointmentDto)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }

            ValidateAppointmentDto(appointmentDto);
            _mapper.Map(appointmentDto, appointment);
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
        }

        public async Task CancelAppointmentAsync(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }

            appointment.Status = Status.Cancelled;
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be less than or equal to end date.");
            }

            var appointments = await _db.Appointments
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .Include(a => a.Patient)
                .Include(a => a.User)
                .ToListAsync();

            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

        public async Task<List<AppointmentDTO>> GetAppointmentsByUserIdAsync(int userId)
        {
            var appointments = await _db.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Patient)
                .ToListAsync();

            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

        private void ValidateAppointmentDto(AppointmentDTO appointmentDto)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(appointmentDto);
            if (!Validator.TryValidateObject(appointmentDto, context, validationResults, true))
            {
                throw new ValidationException(string.Join(", ", validationResults.Select(r => r.ErrorMessage)));
            }
        }
    }
}
