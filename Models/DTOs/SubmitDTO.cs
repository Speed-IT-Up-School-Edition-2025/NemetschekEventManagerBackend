using NemetschekEventManagerBackend.Models.JSON;
using System.ComponentModel.DataAnnotations;

namespace NemetschekEventManagerBackend.Models.DTOs
{
    public class SubmitSummaryDto
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public DateTime? Date { get; set; }
        public IList<Submission>? Submissions { get; set; }
    }

    public class CreateSubmitDto
    {
        public IList<Submission>? Submissions { get; set; }
    }

    public class UpdateSubmitDto
    {
        public IList<Submission>? Submissions { get; set; }
    }
}
