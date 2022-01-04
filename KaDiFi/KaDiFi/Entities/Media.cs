using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class Media
    {

        public Media()
        {
            Likes = 0;
            DisLikes = 0;
            //ViewsCount = 0;
        }

        public string Id { get; set; } // GUID
        public string FriendlyName { get; set; }
        public string MediaName { get; set; }

        public int Type { get; set; } // MediaTypes Enum
        public string PublisherId { get; set; } // UserId
        public string SourcePath { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; } // MediaCategories Enum

        public int Likes { get; set; }
        public int DisLikes { get; set; }
        //public int ViewsCount { get; set; }


        [ForeignKey("PublisherId")]
        public User Users { get; set; }
    }
    [NotMapped]
    public class MediaResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
    }


}