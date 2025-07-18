﻿using Microsoft.AspNetCore.Identity.UI.Services;
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
        if(!_context.Events.Any(e => e.Id == eventId))
            return null;

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

    public async Task<bool> RemoveById(int eventId, IEmailSender emailSender)
    {
        var ev = await _context.Events
            .Include(e => e.Submissions)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null)
            return false;

        var validUsers = ev.Submissions?
            .Where(s => !string.IsNullOrWhiteSpace(s.User?.Email))
            .Select(s => s.User!)
            .Distinct(); // In case multiple submissions from same user

        if (validUsers != null)
        {
            foreach (var user in validUsers)
            {
                var email = user.Email!;
                var userName = email.Split('@')[0];

                await emailSender.SendEmailAsync(
                    email,
                    $"Отменено събитие: {ev.Name}",
                    $@"
                    <html>
                        <body>
                            <p>Здравейте, {userName},</p>
                            <p>Съжаляваме да Ви уведомим, че събитието <strong>{ev.Name}</strong>, планирано за {ev.Date:dd.MM.yyyy}, беше отменено.</p>
                            <p>Поради това Вашата регистрация за събитието беше <strong>изтрита</strong>.</p>
                            <p>Ако имате въпроси, моля свържете се с администратор.</p>
                            <p>С уважение,<br>Екипът по управление на събития</p>
                        </body>
                    </html>");
            }
        }

        _context.Events.Remove(ev);
        return await _context.SaveChangesAsync() > 0;
    }


    public async Task<bool> Update(int eventId, UpdateEventDto dto, IEmailSender emailSender)
    {
        var ev = await _context.Events
            .Include(e => e.Submissions)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null)
            return false;

        var validUsers = ev.Submissions?
            .Where(s => !string.IsNullOrWhiteSpace(s.User?.Email))
            .Select(s => s.User!)
            .Distinct();

        if (validUsers != null)
        {
            foreach (var user in validUsers)
            {
                var email = user.Email!;
                var userName = email.Split('@')[0];

                await emailSender.SendEmailAsync(
                    email,
                    $"Обновено събитие: {ev.Name}",
                    $@"
                    <html>
                        <body>
                            <p>Здравейте, {userName},</p>
                            <p>Искаме да Ви уведомим, че събитието <strong>{ev.Name}</strong>, планирано за {ev.Date:dd.MM.yyyy}, беше <strong>обновено</strong>.</p>
                            <p>Поради направените промени, Вашата предишна регистрация беше <strong>изтрита</strong>. Моля, регистрирайте се отново, ако желаете да участвате.</p>
                            <p>Ако имате въпроси, не се колебайте да се свържете с администратор.</p>
                            <p>С уважение,<br>Екипът по управление на събития</p>
                        </body>
                    </html>");
            }
            _context.Submits.RemoveRange(ev.Submissions);
        }

        EventMapper.UpdateEntity(ev, dto);
        _context.Events.Update(ev);

        return await _context.SaveChangesAsync() > 0;
    }

    public bool Exists(int eventId)
    {
        return _context.Events.Any(e => e.Id == eventId);
    }
}
