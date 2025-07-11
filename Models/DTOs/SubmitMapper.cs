using NemetschekEventManagerBackend.Models.JSON;

namespace NemetschekEventManagerBackend.Models.DTOs
{
    public static class SubmitMapper
    {
        public static Submit ToEntity(int eventId, string userId, CreateSubmitDto dto)
        {
            return new Submit
            {
                EventId = eventId,
                UserId = userId,
                Date = DateTime.UtcNow,
                Submissions = dto.Submissions?.Select(s => new Submission
                {
                    Id = s.Id,
                    Name = s.Name,
                    Options = s.Options
                }).ToList(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(Submit submit, UpdateSubmitDto dto)
        {
            if (dto.Submissions != null)
            {
                submit.Submissions = dto.Submissions.Select(s => new Submission
                {
                    Id = s.Id,
                    Name = s.Name,
                    Options = s.Options
                }).ToList();
                submit.Date = DateTime.UtcNow;
                submit.UpdatedAt = DateTime.UtcNow;
            }
        }

        public static SubmitSummaryDto ToSummaryDto(this Submit submit)
        {
            return new SubmitSummaryDto
            {
                UserId = submit.UserId,
                Email = submit.User?.Email,
                Date = submit.Date,
                Submissions = submit.Submissions
            };
        }
    }
}
