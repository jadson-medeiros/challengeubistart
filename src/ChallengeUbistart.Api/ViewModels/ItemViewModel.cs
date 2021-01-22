using System;
using System.ComponentModel.DataAnnotations;

namespace ChallengeUbistart.Api.ViewModels
{
    public class ItemViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The 'Description' is required")]
        public string Description { get; set; }
    }
}
