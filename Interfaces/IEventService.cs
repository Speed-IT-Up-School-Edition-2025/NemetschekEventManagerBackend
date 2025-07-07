using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Interfaces
{
    public interface IEventService
    {
        bool Create(string name, string description, DateTime? date, DateTime? signUpEndDate, string location);
        Event? GetEventById(int eventId);
        List<Event> GetEvents();
        List<Event> GetEvents(string searchName, DateTime? date, bool? activeOnly);
        bool RemoveById(int eventId);
        bool Update(int eventId, string name, string description, DateTime? date, DateTime? signUpEndDate, string location);
        void UpdateEvent(Event ev);
    }
}
