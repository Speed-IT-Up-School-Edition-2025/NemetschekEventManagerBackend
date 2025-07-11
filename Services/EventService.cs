using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;
using NemetschekEventManagerBackend.Models.JSON;
using NemetschekEventManagerBackend.Interfaces;
using System.Threading.Tasks;
using System.Globalization;

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

    public List<EventSummaryDto> GetJoinedEvents(string userId)
    {
        return _context.Submits
            .Include(s => s.Event)
            .ThenInclude(e => e.Submissions)
            .Where(s => s.UserId == userId && s.Event != null)
            .Select(s => s.Event!)
            .Select(e => e.ToSummaryDto())
            .ToList();
    }

    public List<EventSummaryDto> GetEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, bool alphabetical = false, bool sortDescending = false)
    {
        // Load events from the database
        List<Event> events = _context.Events.Include(e => e.Submissions).ToList();

        // Filter by date range
        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            events = events
                .Where(e => e.Date.HasValue && e.Date.Value.Date >= from)
                .ToList();
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date;
            events = events
                .Where(e => e.Date.HasValue && e.Date.Value.Date <= to)
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

        // Sorting
        if (alphabetical)
        {
            var bulgarianCulture = new CultureInfo("bg-BG");
            var comparer = StringComparer.Create(bulgarianCulture, ignoreCase: true);

            events = sortDescending
                ? events.OrderByDescending(e => e.Name, comparer).ToList()
                : events.OrderBy(e => e.Name, comparer).ToList();
        }
        else
        {
            // Default sort: Date descending (most recent to oldest)
            events = sortDescending
                ? events.OrderByDescending(e => e.Date).ToList()
                : events.OrderBy(e => e.Date).ToList();
        }

        // Convert to DTOs
        return events.Select(e => e.ToSummaryDto()).ToList();
    }

    public async Task<bool> RemoveById(int eventId, IEmailSender _emailSender)
    {
        var ev = await _context.Events
            .Include(e => e.Submissions!)  // Add null-forgiving operator here
            .ThenInclude(s => s!.User)     // Add null-forgiving operator here too
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null)
            return false;

        // Send notifications to all users who submitted to this event
        foreach (var submission in ev.Submissions!)
        {
            if (submission.User == null || string.IsNullOrEmpty(submission.User.Email))
                continue;

            // Extract username from email (everything before '@')
            var email = submission.User.Email;
            var userName = email.Contains('@')
                ? email.Substring(0, email.IndexOf('@'))
                : "User";

            // Use the Send method instead of SendEmailAsync
            await _emailSender.SendEmailAsync(
                email: email,
                subject: $"Event Cancelled: {ev.Name}",
                htmlMessage: $@"
                    <html>
                        <body>
                            <p>Dear {userName},</p>
                            <p>We regret to inform you that the event <strong>{ev.Name}</strong> scheduled for {ev.Date:MMMM d, yyyy} has been cancelled.</p>
                            <p>As a result, your submission for this event has been <strong>permanently deleted</strong>.</p>
                            <p>If you have any questions, please contact an administrator.</p>
                            <p>Sincerely,<br>Event Management Team</p>
                        </body>
                    </html>"
            );
        }

        // Remove the event
        _context.Events.Remove(ev);
        return await _context.SaveChangesAsync() != 0;
    }

    public bool Update(int eventId, UpdateEventDto dto)
    {
        var ev = GetEventById(eventId);
        if (ev == null) return false;

        EventMapper.UpdateEntity(ev, dto);

        _context.Events.Update(ev);
        return _context.SaveChanges() != 0;
    }
    public bool Exists(int eventId)
    {
        return _context.Events.Any(e => e.Id == eventId);
    }
}
