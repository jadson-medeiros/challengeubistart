using System;

namespace ChallengeUbistart.Business.Models
{
    public class Item : Entity
    {
        public Guid ClientId { get; set; }

        public string Description { get; set; }
        
        public ItemStatus ItemStatus { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime FinishedAt { get; set; }

        /* EF Relations */
        public Client Client { get; set; }
    }
}