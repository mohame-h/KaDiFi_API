using System;

namespace KaDiFi.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreatedAt = DateTime.Now;
            IsActive = true;
        }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
