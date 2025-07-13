using Microsoft.AspNetCore.Identity.UI.Services;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;

namespace NemetschekEventManagerBackend
{
    public interface IEventService
    {
            bool Create(Event newEvent);
            Event? GetEventById(int eventId);
            EventDetailsDto? GetEventById(int eventId, string userId);
            List<EventSummaryDto> GetEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, string userId, bool alphabetical = false, bool sortDescending = false);
            List<EventSummaryDto> GetJoinedEvents(string userId);
            Task<bool> RemoveById(int eventId, IEmailSender _emailSender);
            bool Update(int eventId, UpdateEventDto dto);
            bool Exists(int eventId);
    }
}