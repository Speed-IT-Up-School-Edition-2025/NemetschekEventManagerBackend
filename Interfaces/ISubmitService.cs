
using Microsoft.AspNetCore.Identity.UI.Services;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;

namespace NemetschekEventManagerBackend
{
    public interface ISubmitService
    {
        List<SubmitSummaryDto> GetSubmitsByEventId(int eventId);
        Submit? GetSubmitByEventAndUser(int eventId, string userId);
        IResult Create(int eventId, string userId, CreateSubmitDto dto);
        IResult UpdateSubmission(int eventId, string userId, UpdateSubmitDto dto);
        Task<bool> RemoveUserFromEvent(int eventId, string userId, IEmailSender emailSender);
        Task<bool> AdminRemoveUserFromEvent(int eventId, string userId, IEmailSender emailSender);
    }

}