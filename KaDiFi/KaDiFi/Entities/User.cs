﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class User : BaseEntity
    {
        public string Id { get; set; } // GUID
        [MinLength(3, ErrorMessage = "At least 3 Digit")]
        public string Name { get; set; }
        [Range(12, 100, ErrorMessage = "Invalid Age")]
        public int Age { get; set; }
        [MinLength(5, ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [MinLength(5, ErrorMessage = "At least 5 Digits")]
        public string Password { get; set; } // Hashed
        public int RoleId { get; set; } // UserRoles Enum
        public bool TermsFlag { get; set; } // Default is true
    }
  
  

}
