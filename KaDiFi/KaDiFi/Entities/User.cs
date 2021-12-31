using System.ComponentModel.DataAnnotations;

namespace KaDiFi.Entities
{
    public class User
    {
        public string Id { get; set; } // GUID
        [MinLength(1, ErrorMessage = "At least  1 Digit")]
        public string FirstName { get; set; }
        [MinLength(1, ErrorMessage = "At least  1 Digit")]
        public string LastName { get; set; }
        [Range(12, 100, ErrorMessage = "Invalid Age")]
        public int Age { get; set; }
        [MinLength(5, ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [MinLength(5, ErrorMessage = "At least 5 Digits")]
        public string Password { get; set; } // Hashed
        public int RoleId { get; set; } // From Roles Enum
    }

}
