using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class MediaComment : BaseEntity
    {

        public MediaComment()
        {
        }

        public string Id { get; set; } // GUID
        [MinLength(1, ErrorMessage = "At least 1 Digit")]
        public string Body { get; set; }
        //public DateTime Time { get; set; }
        public string CommenterId { get; set; }
        [ForeignKey("CommenterId")]
        public User Users { get; set; }

        public string MediaId { get; set; }
        [ForeignKey("MediaId")]
        public Media Medias { get; set; }
    }

}