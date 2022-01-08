using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class MediaViews: BaseEntity
    {
        public string Id { get; set; } // GUID
        public string MediaId { get; set; }
        public string UserId { get; set; }

        public int? React { get; set; } // MediaReactTypes


        [ForeignKey("MediaId")]
        public Media Media { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }


}