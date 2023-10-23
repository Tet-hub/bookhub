using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.ServiceModels
{
    public class AdminViewModel
    {
        [Required(ErrorMessage = "Admin Name is required.")]
        public string AdminName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string AdminEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateTime Dob { get; set; }
    }
}
