using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReCycle.Models.Requests.User
{
    public class UserPasswordUpdateRequest
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "A Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Your password must contain at least one uppercase letter, one lowercase letter, one number and one special character, between 8 to 15 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your new password.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Please ensure your passwords match. Check them and try again.")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

    }
}
