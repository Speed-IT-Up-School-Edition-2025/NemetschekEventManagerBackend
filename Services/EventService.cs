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
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Events.Add(ev);
        return _context.SaveChanges() != 0;
    }

    public Event? GetEventById(int eventId)
    {
        return _context.Events
            .Include(e => e.Fields)
            .Include(e => e.Submissions)
            .FirstOrDefault(e => e.Id == eventId);
    }

    public List<Event> GetEvents()
    {
        return _context.Events
            .Include(e => e.Fields)
            .Include(e => e.Submissions)
            .ToList();
    }

    public List<Event> GetEvents(string searchName, DateTime? date, bool? activeOnly)
    {
        var query = _context.Events
            .Include(e => e.Fields)
            .Include(e => e.Submissions)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(e => e.Name != null && e.Name.ToLower().Contains(searchName.ToLower()));
        }

        if (date.HasValue)
        {
            query = query.Where(e => e.Date.HasValue && e.Date.Value.Date == date.Value.Date);
        }

        if (activeOnly == true)
        {
            var now = DateTime.UtcNow;
            query = query.Where(e => e.SignUpEndDate.HasValue && e.SignUpEndDate.Value > now);
        }

        return query.ToList();
    }

    public bool RemoveById(int eventId)
    {
        var ev = GetEventById(eventId);
        if (ev == null)
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
