using NemetschekEventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NemetschekEventManagerBackend.Models
{
    public class Submit
    {
        [Key, Column(Order = 0)]
        public int EventId { get; set; }
        [Key, Column(Order = 1)]
        public int UserId { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public IList<Submission>? Submissions { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
