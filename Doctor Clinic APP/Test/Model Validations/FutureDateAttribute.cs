using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event_Planinng_System_DAL.Model_Validations
{
    internal class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
        {
            ErrorMessage = "The date must be in the future.";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true; // Assuming that null values are handled elsewhere, like with [Required]
            }

            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }

            return false;
        }
    }
}
