using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class Media : BaseEntity
    {
        public string Id { get; set; } // GUID

        public string Title { get; set; }

        public string CoverSource { get; set; }
        public string Source { get; set; }

        public string Description { get; set; }

        public int Type { get; set; } // MediaTypes Enum
        public int CategoryId { get; set; } // MediaCategories Enum

        public string PublisherId { get; set; } // UserId


        [ForeignKey("PublisherId")]
        public User Users { get; set; }
    }


}