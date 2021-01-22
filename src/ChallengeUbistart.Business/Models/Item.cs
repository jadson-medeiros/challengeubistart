using System;

namespace ChallengeUbistart.Business.Models
{
    public class Item : Entity
    {
        public string Description { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime FinishedAt { get; set; }
    }
}