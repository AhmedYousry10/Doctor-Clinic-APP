using Doctor_CLinic_API.Enums;
using System.ComponentModel.DataAnnotations;

namespace Doctor_CLinic_API.DTO
{
    public class RegisterDTO
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%?&])[A-Za-z\d@$!%?&]{8,}$", ErrorMessage = "Invaild Password")]
        [StringLength(100, ErrorMessage = "Password length must be between 8 and 100 characters.", MinimumLength = 8)]
        public string Password { get; set; }

    }
}
