using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KaDiFi.Entities
{
    public class PaginationDTO
    {
        public int itemsCount { get; set; }
        public int pageNumber { get; set; }
    }

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
        public int commentsTotalCount { get; set; }
        public int repliesTotalCount { get; set; }
    }

    public class AddOrRemoveReactDTO
    {
        public string mediaId { get; set; }
        public int reactTypeId { get; set; }
    }

    public class AddMediaCommentDTO
    {
        public string mediaId { get; set; }
        public string commentText { get; set; }
    }
    public class EditMediaCommentDTO
    {
        public string commentId { get; set; }
        public string commentText { get; set; }
    }

    public class GetMediaCommentsDTO : PaginationDTO
    {
        public string mediaId { get; set; }
    }

    public class AddMediaReplyDTO
    {
        public string commentId { get; set; }
        public string replyText { get; set; }
    }
    public class EditTextDTO
    {
        public string id { get; set; }
        public string text { get; set; }
    }
    public class GetMediaRepliesDTO : PaginationDTO
    {
        public string commentId { get; set; }
    }

}