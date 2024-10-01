using Doctor_CLinic_API.DTO;
using Doctor_CLinic_API.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Doctor_CLinic_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // GET: api/Appointment
        [HttpGet]
        [SwaggerOperation(Summary = "Get all appointments", Description = "Get all appointments")]
        [SwaggerResponse(200, "Appointments found", typeof(List<AppointmentDTO>))]
        [SwaggerResponse(404, "No appointments found")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAppointments(int pageNumber = 1, int pageSize = 10)
        {
            var appointments = await _appointmentService.GetAppointmentsAsync(pageNumber, pageSize);
            if (appointments == null || appointments.Count == 0)
            {
                return NotFound("No appointments found.");
            }
            return Ok(appointments);
        }

        // GET: api/Appointment/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get an appointment by id", Description = "Get an appointment by id")]
        [SwaggerResponse(200, "Appointment found", typeof(AppointmentDTO))]
        [SwaggerResponse(404, "Appointment not found")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                return Ok(appointment);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Appointment not found.");
            }
        }

        // POST: api/Appointment
        [HttpPost]
        [SwaggerOperation(Summary = "Add an appointment", Description = "Add an appointment")]
        [SwaggerResponse(201, "Appointment created", typeof(AppointmentDTO))]
        [SwaggerResponse(400, "Invalid appointment details")]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDTO appointmentDto, [FromHeader] int userId)
        {
            try
            {
                await _appointmentService.AddAppointmentAsync(appointmentDto, userId);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentDto.Id }, appointmentDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Appointment/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an appointment", Description = "Update an appointment")]
        [SwaggerResponse(204, "Appointment updated")]
        [SwaggerResponse(400, "Invalid appointment details")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDto)
        {
            try
            {
                await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Appointment not found.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Appointment/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete an appointment", Description = "Delete an appointment")]
        [SwaggerResponse(204, "Appointment deleted")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            try
            {
                await _appointmentService.CancelAppointmentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Appointment not found.");
            }
        }

        // GET: api/Appointment/date-range
        [HttpGet("date-range")]
        [SwaggerOperation(Summary = "Get appointments by date range", Description = "Get appointments by date range")]
        [SwaggerResponse(200, "Appointments found", typeof(List<AppointmentDTO>))]
        [SwaggerResponse(400, "Invalid date range")]
        [SwaggerResponse(404, "No appointments found")]
        public async Task<IActionResult> GetAppointmentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date must be less than or equal to end date.");
            }

            var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate);
            if (appointments == null || appointments.Count == 0)
            {
                return NotFound("No appointments found.");
            }
            return Ok(appointments);
        }

        // GET: api/Appointment/user
        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Get appointments by user", Description = "Get appointments by user")]
        [SwaggerResponse(200, "Appointments found", typeof(List<AppointmentDTO>))]
        [SwaggerResponse(404, "No appointments found")]
        public async Task<IActionResult> GetAppointmentsByUserId(int userId)
        {
            var appointments = await _appointmentService.GetAppointmentsByUserIdAsync(userId);
            if (appointments == null || appointments.Count == 0)
            {
                return NotFound("No appointments found.");
            }
            return Ok(appointments);
        }
    }
}
