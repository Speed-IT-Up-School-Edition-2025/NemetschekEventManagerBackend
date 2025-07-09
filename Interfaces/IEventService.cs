using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;

namespace NemetschekEventManagerBackend.Interfaces
{
    public interface IEventService
    {
        bool Create(Event newEvent);
        Event? GetEventById(int eventId);
        List<EventSummaryDto> GetEvents();
        List<Event> GetEvents(string searchName, DateTime? date, bool? activeOnly);
        bool RemoveById(int eventId);
        bool Update(int eventId, UpdateEventDto dto);
    }
}
