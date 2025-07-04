using System.ComponentModel.DataAnnotations;

namespace NemetschekEventManagerBackend.Models.JSON
{
    public class Submission
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IList<string>? Options { get; set; }
    }
}
