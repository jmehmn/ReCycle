using ReCycle.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ReCycle.Models.Requests
{
    public class UserUpdateRequest : IModelIdentifier
    {
        public int Id { get; set; }

        [Required, StringLength(maximumLength: 100)]
        [EmailAddress(ErrorMessage = "The email address entered was not found in our system.")]
        public string Email { get; set; }

        [Required]
        public bool IsConfirmed { get; set; }

        [Required]
        public int UserStatusId { get; set; }
    }
}