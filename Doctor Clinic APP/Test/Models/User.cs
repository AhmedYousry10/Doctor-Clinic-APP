using Doctor_CLinic_API.Enums;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_CLinic_API.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "enter a string between 3 and 50")]
        public string Name { get; set; }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<Patient> Patients { get; set; } = new List<Patient>();

    }
}
 
