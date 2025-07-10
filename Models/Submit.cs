using NemetschekEventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NemetschekEventManagerBackend.Models
{
    public class Submit
    {
        [Key]
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
        [JsonIgnore]
        public Event? Event { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User? User { get; set; }
    }
}
