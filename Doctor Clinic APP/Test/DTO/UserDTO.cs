using System.ComponentModel.DataAnnotations;

namespace Doctor_CLinic_API.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Enter a string between 3 and 50.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%?&])[A-Za-z\d@$!%?&]{8,}$", ErrorMessage = "Invaild Password")]
        [StringLength(100, ErrorMessage = "Password length must be between 8 and 100 characters.", MinimumLength = 8)]
        public string Password { get; set; }

        public List<int> AppointmentIds { get; set; } = new List<int>();

        public List<int> PatientIds { get; set; } = new List<int>();
    }
}
