using NemetschekEventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NemetschekEventManagerBackend.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? SignUpDeadline { get; set; }
        public string? Location { get; set; }
        public IList<Field>? Fields { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [JsonIgnore]
        public ICollection<Submit>? Submissions { get; set; }
    }
}
