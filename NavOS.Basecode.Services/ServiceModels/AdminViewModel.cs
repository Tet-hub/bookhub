using Microsoft.AspNetCore.Http;
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
        public string AdminId {  get; set; }
        public string Token { get; set; }

        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Don't leave it blank.")]
        [Required(ErrorMessage = "Admin Name is required.")]
        public string AdminName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Don't leave it blank.")]
        public string AdminEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        //[RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Don't leave it blank.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        //[RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Don't leave it blank.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "Invalid phone number format. [Format: 09012345678].")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateTime Dob { get; set; }

		[Required(ErrorMessage = "Role is required.")]
		public string Role { get; set; }

		//[Required(ErrorMessage = "Admin Profile is required.")]
		public IFormFile AdminProfile { get; set; }

        public string ImageUrl { get; set; }

	}
}
