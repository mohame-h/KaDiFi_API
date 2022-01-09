using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class MediaCommentReply : BaseEntity
    {
        public string Id { get; set; } // GUID

        [MinLength(1, ErrorMessage = "At least 1 Digit")]
        public string Body { get; set; }

        public string ReplierId { get; set; }
        [ForeignKey("ReplierId")]
        public User Users { get; set; }

        public string CommentId { get; set; }
        [ForeignKey("CommentId")]
        public MediaComment Comments { get; set; }
    }


}