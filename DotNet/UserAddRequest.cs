using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReCycle.Models.Requests
{
    public class UserAddRequest
    {
        [Required(ErrorMessage = "An Email address is required to register an account."), StringLength(maximumLength: 100, ErrorMessage ="Your email must be shorter than 100 characters.")]
        [EmailAddress(ErrorMessage ="The email address entered is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Password is required to complete registration.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Your password must contain at least one uppercase letter, one lowercase letter, one number and one special character, between 8 to 15 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm your password to complete registration.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Please ensure your passwords match. Check them and try again.")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Please select the type of account you are registering")]
        public string UserRole { get; set; }
    }
}
