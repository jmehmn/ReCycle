using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReCycle.Models.Requests
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "An Email address is required to log in.")]
        [EmailAddress(ErrorMessage = "The email address entered was not found in our system.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Password is required to log in.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "The password entered was not found in our system. Please ensure your password is correct and meets the requirements for password length and complexity.")]
        public string Password { get; set; }
    }
}
