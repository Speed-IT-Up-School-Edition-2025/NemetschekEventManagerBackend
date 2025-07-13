using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;
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

    public EventDetailsDto? GetEventById(int eventId, string userId)
    {
        return _context.Events.Include(e => e.Submissions).First(e => e.Id == eventId).ToDetailsDto(userId);
    }

    public List<Event> GetEvents()
    {
        return _context.Events
            .AsNoTracking()
            .ToList();
    }

    public List<EventSummaryDto> GetJoinedEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, string userId, bool alphabetical = false, bool sortDescending = false)
    {
        List<Event> events = _context.Submits
            .Include(s => s.Event)
            .ThenInclude(e => e.Submissions)
            .Where(s => s.UserId == userId && s.Event != null)
            .Select(s => s.Event!)
            .ToList();

        // Apply filters
        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            events = events.Where(e => e.Date.HasValue && e.Date.Value.Date >= from).ToList();
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date;
            events = events.Where(e => e.Date.HasValue && e.Date.Value.Date <= to).ToList();
        }

        if (activeOnly == true)
        {
            var now = DateTime.UtcNow;
            events = events.Where(e => e.SignUpDeadline.HasValue && e.SignUpDeadline.Value > now).ToList();
        }

        // Determine whether to apply the custom sort
        bool filtersApplied = fromDate.HasValue || toDate.HasValue || activeOnly == true;
        var nowTime = DateTime.UtcNow;

        // Sorting
        if (!filtersApplied)
        {
            // Apply default sort: active w/ spots → active full → inactive
            events = events
                .OrderBy(e =>
                {
                    bool isActive = e.SignUpDeadline.HasValue && e.SignUpDeadline.Value > nowTime;
                    bool hasSpots = e.PeopleLimit == null || e.Submissions.Count < e.PeopleLimit;
                    return isActive ? (hasSpots ? 0 : 1) : 2;
                })
                .ThenBy(e => alphabetical ? (IsEnglish(e.Name) ? 0 : 1) : 0)
                .ThenBy(e => alphabetical ? e.Name : null)
                .ThenBy(e => !alphabetical ? e.Date : null)
                .ToList();
        }
        else
        {
            // Skip custom status-based sort when filters are applied
            if (alphabetical)
            {
                events = events
                    .OrderBy(e => IsEnglish(e.Name) ? 0 : 1)
                    .ThenBy(e => e.Name, StringComparer.Create(new CultureInfo("bg-BG"), ignoreCase: true))
                    .ToList();
            }
            else
            {
                events = events
                    .OrderBy(e => e.Date)
                    .ToList();
            }
        }

        if (sortDescending)
        {
            events.Reverse();
        }

        return events.Select(e => e.ToSummaryDto(userId)).ToList();
    }

    public List<EventSummaryDto> GetEvents(DateTime? fromDate, DateTime? toDate, bool? activeOnly, string userId, bool alphabetical = false, bool sortDescending = false)
    {
        List<Event> events = _context.Events.Include(e => e.Submissions).ToList();

        // Apply filters
        if (fromDate.HasValue)
        {
            var from = fromDate.Value.Date;
            events = events.Where(e => e.Date.HasValue && e.Date.Value.Date >= from).ToList();
        }

        if (toDate.HasValue)
        {
            var to = toDate.Value.Date;
            events = events.Where(e => e.Date.HasValue && e.Date.Value.Date <= to).ToList();
        }

        if (activeOnly == true)
        {
            var now = DateTime.UtcNow;
            events = events.Where(e => e.SignUpDeadline.HasValue && e.SignUpDeadline.Value > now).ToList();
        }

        // Determine whether to apply the custom sort
        bool filtersApplied = fromDate.HasValue || toDate.HasValue || activeOnly == true;
        var nowTime = DateTime.UtcNow;

        // Sorting
        if (!filtersApplied)
        {
            // Apply default sort: active w/ spots → active full → inactive
            events = events
                .OrderBy(e =>
                {
                    bool isActive = e.SignUpDeadline.HasValue && e.SignUpDeadline.Value > nowTime;
                    bool hasSpots = e.PeopleLimit == null || e.Submissions.Count < e.PeopleLimit;
                    return isActive ? (hasSpots ? 0 : 1) : 2;
                })
                .ThenBy(e => alphabetical ? (IsEnglish(e.Name) ? 0 : 1) : 0)
                .ThenBy(e => alphabetical ? e.Name : null)
                .ThenBy(e => !alphabetical ? e.Date : null)
                .ToList();
        }
        else
        {
            // Skip custom status-based sort when filters are applied
            if (alphabetical)
            {
                events = events
                    .OrderBy(e => IsEnglish(e.Name) ? 0 : 1)
                    .ThenBy(e => e.Name, StringComparer.Create(new CultureInfo("bg-BG"), ignoreCase: true))
                    .ToList();
            }
            else
            {
                events = events
                    .OrderBy(e => e.Date)
                    .ToList();
            }
        }

        if (sortDescending)
        {
            events.Reverse();
        }

        return events.Select(e => e.ToSummaryDto(userId)).ToList();
    }

    private bool IsEnglish(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        char firstChar = input.Trim()[0];
        return (firstChar >= 'A' && firstChar <= 'Z') || (firstChar >= 'a' && firstChar <= 'z');
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
