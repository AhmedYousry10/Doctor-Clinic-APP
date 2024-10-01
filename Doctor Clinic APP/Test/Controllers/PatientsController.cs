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
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // Get all patients
        [HttpGet]
        [SwaggerOperation(Summary = "Get all patients", Description = "Retrieve a paginated list of patients")]
        [SwaggerResponse(200, "Patients found", typeof(List<PatientDTO>))]
        [SwaggerResponse(404, "No patients found")]
        public async Task<ActionResult<List<PatientDTO>>> GetPatients(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 || pageSize > 100 ? 10 : pageSize;

            var patients = await _patientService.GetPatientsAsync(pageNumber, pageSize);
            if (patients == null || patients.Count == 0)
            {
                return NotFound("No patients found");
            }
            return Ok(patients);
        }

        // Get a patient by ID
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a patient by ID", Description = "Retrieve details of a specific patient by their ID")]
        [SwaggerResponse(200, "Patient found", typeof(PatientDTO))]
        [SwaggerResponse(404, "Patient not found")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(id);
                return Ok(patient);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Patient not found.");
            }
        }

        // Add a patient
        [HttpPost]
        [SwaggerOperation(Summary = "Add a patient", Description = "Create a new patient record")]
        [SwaggerResponse(201, "Patient created", typeof(PatientDTO))]
        public async Task<ActionResult<PatientDTO>> AddPatient([FromBody] PatientDTO patientDto, [FromHeader] int userId)
        {
            try
            {
                await _patientService.AddPatientAsync(patientDto, userId);
                return CreatedAtAction(nameof(GetPatient), new { id = patientDto.Id }, patientDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Update a patient
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a patient", Description = "Modify an existing patient record")]
        [SwaggerResponse(204, "Patient updated")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientDTO patientDto, [FromHeader] int userId)
        {
            try
            {
                await _patientService.UpdatePatientAsync(id, patientDto, userId);
                return Ok("Patient Updated");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Patient not found.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete a patient
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a patient", Description = "Remove a patient record")]
        [SwaggerResponse(204, "Patient deleted")]
        [SwaggerResponse(404, "Patient not found")]
        public async Task<IActionResult> DeletePatient(int id, [FromHeader] int userId)
        {
            try
            {
                await _patientService.DeletePatientAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Patient not found.");
            }
        }

        // Get patients by date range
        [HttpGet("dateRange")]
        [SwaggerOperation(Summary = "Get patients by date range", Description = "Retrieve a list of patients within a specified date range")]
        [SwaggerResponse(200, "Patients found", typeof(List<PatientDTO>))]
        [SwaggerResponse(404, "No patients found")]
        public async Task<ActionResult<List<PatientDTO>>> GetPatientsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var patients = await _patientService.GetPatientsByDateRangeAsync(startDate, endDate);
            if (patients == null || patients.Count == 0)
            {
                return NotFound("No patients found");
            }
            return Ok(patients);
        }
    }
}
