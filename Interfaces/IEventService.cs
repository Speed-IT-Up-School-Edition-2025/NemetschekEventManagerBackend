using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;

namespace NemetschekEventManagerBackend.Interfaces
{
    public interface IEventService
    {
        bool Create(Event newEvent);
        Event? GetEventById(int eventId);
        List<EventSummaryDto> GetEvents();
        List<EventSummaryDto> GetEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, bool alphabetical = false, bool sortDescending = false);
        bool RemoveById(int eventId);
        bool Update(int eventId, UpdateEventDto dto);
        bool Exists(int eventId);
    }
}
