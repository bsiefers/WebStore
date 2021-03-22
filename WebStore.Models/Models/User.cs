using System;
using System.Collections.Generic;
using System.Text;

namespace WebStore.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string UserRole { get; set; }
        public string Salt { get; set; }
    }
}
