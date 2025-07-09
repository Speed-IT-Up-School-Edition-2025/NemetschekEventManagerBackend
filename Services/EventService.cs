using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;

public class EventService : IEventService
{
    private readonly EventDbContext _context;

    public EventService(EventDbContext context)
    {
        _context = context;
    }
    public bool Create(Event newEvent)
    {
        _context.Events.Add(newEvent);
        return _context.SaveChanges() != 0;
    }

    public Event? GetEventById(int eventId)
    {
        return _context.Events.Find(eventId);
    }

    public List<EventSummaryDto> GetEvents()
    {
        return _context.Events
            .AsNoTracking()
            .Select(e => e.ToSummaryDto())
            .ToList();
    }


    public List<Event> GetEvents(string searchName, DateTime? date, bool? activeOnly)
    {
        // Load events with related data from the database
        List<Event> events = _context.Events.ToList();

        // Filter by event name
        if (!string.IsNullOrWhiteSpace(searchName))
        {
            events = events
                .Where(e => e.Name != null && e.Name.ToLower().Contains(searchName.ToLower()))
                .ToList();
        }

        // Filter by exact date
        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            events = events
                .Where(e => e.Date.HasValue && e.Date.Value.Date == targetDate)
                .ToList();
        }

        // Filter by whether the signup period is still active
        if (activeOnly == true)
        {
            var now = DateTime.UtcNow;
            events = events
                .Where(e => e.SignUpDeadline.HasValue && e.SignUpDeadline.Value > now)
                .ToList();
        }

        return events;
    }

    public bool RemoveById(int eventId)
    {
        var ev = GetEventById(eventId);
        if (ev == null)
            return false;

        _context.Events.Remove(ev);
        return _context.SaveChanges() != 0;
    }

    public bool Update(int eventId, UpdateEventDto dto)
    {
        var ev = GetEventById(eventId);
        if (ev == null) return false;

        EventMapper.UpdateEntity(ev, dto);

        _context.Events.Update(ev);
        return _context.SaveChanges() != 0;
    }
}
