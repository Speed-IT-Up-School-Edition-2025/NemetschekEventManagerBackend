using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;

public interface ISubmitService
{
    List<SubmitSummaryDto> GetSubmitsByEventId(int eventId);
    Submit? GetSubmitByEventAndUser(int eventId, string userId);
    bool Create(int eventId, string userId, CreateSubmitDto dto);
    bool UpdateSubmission(int eventId, string userId, UpdateSubmitDto dto);
}
