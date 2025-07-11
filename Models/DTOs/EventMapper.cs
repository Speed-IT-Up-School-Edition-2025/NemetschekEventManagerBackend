using NemetschekEventManagerBackend.Models.JSON;

namespace NemetschekEventManagerBackend.Models.DTOs
{
    public static class EventMapper
    {
        public static Event ToEntity(CreateEventDto dto)
        {
            if (dto.PeopleLimit < 1) { dto.PeopleLimit = null; }

            return new Event
            {
                Name = dto.Name ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                Location = dto.Location ?? string.Empty,
                Date = dto.Date,
                SignUpDeadline = dto.SignUpDeadline ?? dto.Date ?? DateTime.UtcNow,
                PeopleLimit = dto.PeopleLimit,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Fields = dto.Fields?.Select(f => new Field
                {
                    Type = f.Type,
                    Name = f.Name,
                    Options = f.Options,
                    Required = f.Required
                }).ToList()
            };
        }

        public static void UpdateEntity(Event ev, UpdateEventDto dto)
        {
            if (dto.Name != null) ev.Name = dto.Name;
            if (dto.Description != null) ev.Description = dto.Description;
            if (dto.Location != null) ev.Location = dto.Location;
            if (dto.Date.HasValue) ev.Date = dto.Date;
            if (dto.SignUpDeadline.HasValue) ev.SignUpDeadline = dto.SignUpDeadline;
            if (dto.PeopleLimit != null) ev.PeopleLimit = dto.PeopleLimit; 

            // For Fields, you might want to handle add/update/remove carefully
            if (dto.Fields != null)
            {
                // Example: replace all fields with the new list
                ev.Fields = dto.Fields.Select(f => new Field
                {
                    Type = f.Type,
                    Name = f.Name,
                    Options = f.Options,
                    Required = false
                }).ToList();
            }

            ev.UpdatedAt = DateTime.UtcNow;
        }
        public static EventSummaryDto ToSummaryDto(this Event ev)
        {
            return new EventSummaryDto
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                Date = ev.Date,
                SignUpDeadline = ev.SignUpDeadline,
                Location = ev.Location,
                PeopleLimit = ev.PeopleLimit,
                SpotsLeft = ev.PeopleLimit - ev.Submissions!.Count(),
                CreatedAt = ev.CreatedAt,
                UpdatedAt = ev.UpdatedAt
            };
        }

        public static EventDetailsDto ToDetailsDto(this Event ev, string userId)
        {
            return new EventDetailsDto
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                Date = ev.Date,
                SignUpDeadline = ev.SignUpDeadline,
                Location = ev.Location,
                PeopleLimit = ev.PeopleLimit,
                SpotsLeft = ev.PeopleLimit - ev.Submissions!.Count(),
                UserSignedUp = ev.Submissions!.Where(s => s.UserId == userId).Any(),
                CreatedAt = ev.CreatedAt,
                UpdatedAt = ev.UpdatedAt
            };
        }
    }

}
