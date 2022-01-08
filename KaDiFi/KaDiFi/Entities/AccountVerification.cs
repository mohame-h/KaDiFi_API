using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class AccountVerification : BaseEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User Users { get; set; }

        public string Code { get; set; }
    }
}
