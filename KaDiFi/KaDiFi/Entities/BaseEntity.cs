using System;
using System.ComponentModel;

namespace KaDiFi.Entities
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}
