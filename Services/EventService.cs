using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using System;

public class EventService : IEventService
{
    private readonly EventDbContext _context;

    public EventService(EventDbContext context)
    {
        _context = context;
    }

    public bool Create(string name, string description, DateTime? date, DateTime? signUpEndDate, string location)
    {
        Event ev = new Event
        {
            Name = name,
            Description = description,
            Date = date,
            SignUpEndDate = signUpEndDate,
            Location = location,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Events.Add(ev);
        return _context.SaveChanges() != 0;
    }

    public Event? GetEventById(int eventId)
    {
        return _context.Events.Find(eventId);
    }



    public List<Event> GetEvents()
    {
        List<Event> ev = _context.Events.ToList();
        return ev;
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
                .Where(e => e.SignUpEndDate.HasValue && e.SignUpEndDate.Value > now)
                .ToList();
        }

        return events;
    }

  

    public bool RemoveById(int eventId)
    {
        var ev = GetEventById(eventId);
        if (ev == null )
            return false;

        _context.Events.Remove(ev);
        return _context.SaveChanges() != 0;
    }

    public bool Update(int eventId, string name, string description, DateTime? date, DateTime? signUpEndDate, string location)
    {
        var ev = GetEventById(eventId);
        if (ev == null)
            return false;

        ev.Name = name;
        ev.Description = description;
        ev.Date = date;
        ev.SignUpEndDate = signUpEndDate;
        ev.Location = location;
        ev.UpdatedAt = DateTime.UtcNow;

        _context.Events.Update(ev);
        return _context.SaveChanges() != 0;
    }

    public void UpdateEvent(Event ev)
    {
        ev.UpdatedAt = DateTime.UtcNow;
        _context.Events.Update(ev);
        _context.SaveChanges();
    }
}
