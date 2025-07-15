using Microsoft.AspNetCore.Identity;

namespace NemetschekEventManagerBackend.Models
{
    public class User : IdentityUser
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Submit>? Submissions { get; set; }
    }
}
