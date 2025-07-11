using NemetschekEventManagerBackend.Models.JSON;

namespace NemetschekEventManagerBackend.Models.DTOs
{
    public class EventSummaryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? SignUpDeadline { get; set; }
        public string? Location { get; set; }
        public int? PeopleLimit { get; set; }
        public int? SpotsLeft { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class EventDetailsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? SignUpDeadline { get; set; }
        public string? Location { get; set; }
        public int? PeopleLimit { get; set; }
        public int? SpotsLeft { get; set; }
        public bool UserSignedUp { get; set; }
        public IList<Field>? Fields { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateEventDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? SignUpDeadline { get; set; }
        public string? Location { get; set; }
        public int? PeopleLimit { get; set; }
        public IList<Field>? Fields { get; set; }
    }

    public class UpdateEventDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? SignUpDeadline { get; set; }
        public string? Location { get; set; }
        public int? PeopleLimit { get; set; }
        public IList<Field>? Fields { get; set; }
    }

}
