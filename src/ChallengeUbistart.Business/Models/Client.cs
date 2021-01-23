using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeUbistart.Business.Models
{
    public class Client : Entity
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        /* EF Relations */
        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}