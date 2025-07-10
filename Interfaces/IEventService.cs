
using Microsoft.AspNetCore.Identity.UI.Services;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;
namespace NemetschekEventManagerBackend;

public interface IEventService
{
    public interface IEventService
    {
        bool Create(Event newEvent);
        Event? GetEventById(int eventId);
        List<EventSummaryDto> GetEvents();
        List<EventSummaryDto> GetEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, bool alphabetical = false, bool sortDescending = false);
        Task<bool> RemoveById(int eventId, IEmailSender _emailSender);
        bool Update(int eventId, UpdateEventDto dto);
        bool Exists(int eventId);
    }
}
