using NemetschekEventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NemetschekEventManagerBackend.Models
{
    public class Submit
    {
        [Key,]
        public int EventId { get; set; }
        [Key]
        public string? UserId { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public IList<Submission>? Submissions { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }
        [Required]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
