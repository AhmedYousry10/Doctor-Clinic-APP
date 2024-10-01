using Doctor_CLinic_API.Enums;
using Doctor_CLinic_API_DAL.Model_Validations;
using Event_Planinng_System_DAL.Model_Validations;
using System.ComponentModel.DataAnnotations;

namespace Doctor_CLinic_API.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [FutureDate (ErrorMessage = "Appointment date can not be in the past.")]
        public DateTime AppointmentDate { get; set; }

        [MaxLength(500, ErrorMessage = "Notes can't exceed 500 characters.")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public Status Status { get; set; } = Status.Pending;

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientId { get; set; }

        

    }
}
