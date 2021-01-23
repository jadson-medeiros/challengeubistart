using System;
using System.ComponentModel.DataAnnotations;

namespace ChallengeUbistart.Api.ViewModels
{
    public class ItemViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }

        [Required(ErrorMessage = "The 'DueDate' is required")]
        public DateTime DueDate { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "The 'Description' is required")]
        [StringLength(1000, ErrorMessage = "The field {0} need to have between {2} and {1} characters", MinimumLength = 2)]
        public string Description { get; set; }

        public string Status { get; set; }
        public string ClientEmail { get; set; }
    }
}
