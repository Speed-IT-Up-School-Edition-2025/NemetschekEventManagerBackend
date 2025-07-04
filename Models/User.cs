using System.ComponentModel.DataAnnotations;

namespace NemetschekEventManagerBackend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required, MinLength(6)]
        public string? PasswordHash { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public ICollection<Submit>? Submissions { get; set; }
    }
}
