using System;
using System.Collections.Generic;
using System.Text;

namespace ReCycle.Models.Domain
{
    public class User
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public int UserStatusId { get; set; }

        public Role Roles { get; set; }

        public int Id { get; set; }
    }
}