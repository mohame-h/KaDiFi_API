using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaDiFi.Entities
{
    public class AccountDeactivation : BaseEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User Users { get; set; }

        public DateTime Until{ get; set; }
    }
}