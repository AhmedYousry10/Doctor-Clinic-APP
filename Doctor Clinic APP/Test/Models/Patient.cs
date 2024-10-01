using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doctor_CLinic_API.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public bool IsFirstVisit { get; set; } = true;

        public int? UserId { get; set; }
        public User User { get; set; }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
