using NemetschekEventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;

namespace NemetschekEventManagerBackend.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? SignUpEndDate { get; set; }
        public string? Location { get; set; }
        public IList<Field>? Fields { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Submit>? Submissions { get; set; }
    }
}
