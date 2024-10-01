using Doctor_CLinic_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_CLinic_API.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public Status Status { get; set; } = Status.Pending;

        [MaxLength(500)]
        public string Notes { get; set; }

        public int PatientId { get; set; }  
        public Patient Patient { get; set; }


        public int? UserId { get; set; }  
        public User User { get; set; }

    }
}
