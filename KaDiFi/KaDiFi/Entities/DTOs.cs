using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KaDiFi.Entities
{
    public class ChangePasswordDTO
    {
        [MinLength(5, ErrorMessage = "At least 5 Digits")]
        public string oldPassword { get; set; }
        [MinLength(5, ErrorMessage = "At least 5 Digits")]
        public string newPassword { get; set; }
    }

    public class FreezeAccountDTO
    {
        public string password { get; set; }
        public int daysCount { get; set; }
    }

    public class MediaCommentDTO
    {
        public string mediaId { get; set; }
        public string UserId { get; set; }
        public string commentBody { get; set; }
    }

    public class LoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class UserDTO
    {
        [MinLength(3, ErrorMessage = "At least 3 Digit")]
        public string name { get; set; }
        [Range(12, 100, ErrorMessage = "Invalid Age")]
        public int age { get; set; }
        [MinLength(5, ErrorMessage = "Invalid Email")]
        public string email { get; set; }
        [MinLength(5, ErrorMessage = "At least 5 Digits")]
        public string password { get; set; } // Hashed
        [DefaultValue(true)]
        public bool acceptTerms { get; set; }
    }

    public class GetMediaDTO
    {
        public string mediaId { get; set; }
        public int commentsTotalCount{ get; set; }
        public int repliesTotalCount { get; set; }
    }





}
