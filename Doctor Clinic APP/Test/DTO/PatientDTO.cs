using Doctor_CLinic_API_DAL.Model_Validations;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;

namespace Doctor_CLinic_API.DTO
{
    public class PatientDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"(01)[0125][0-9]{8}$", ErrorMessage = "Phone number must be a vaild phone number")]
        public string PhoneNumber { get; set; }

        public bool IsFirstVisit { get; set; } = true;

        /*Include patient appointments*/
        public List<AppointmentDTO> Appointments { get; set; } = new List<AppointmentDTO>();
    }
}
