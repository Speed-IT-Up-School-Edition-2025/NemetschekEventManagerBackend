using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;

public class SubmitService : ISubmitService
{
    private readonly EventDbContext _context;

    public SubmitService(EventDbContext context)
    {
        _context = context;
    }
    public List<SubmitSummaryDto> GetSubmitsByEventId(int eventId)
    {
        return _context.Submits
            .Include(s => s.User)
            .Where(s => s.EventId == eventId)
            .Select(s => SubmitMapper.ToSummaryDto(s))
            .ToList();
    }

    public Submit? GetSubmitByEventAndUser(int eventId, string userId)
    {
        return _context.Submits
            .FirstOrDefault(s => s.EventId == eventId && s.UserId == userId);
    }

    public IResult Create(int eventId, string userId, CreateSubmitDto dto)
    {
        if (_context.Submits.Any(s => s.EventId == eventId && s.UserId == userId))
            return Results.Conflict(new { error = "Потребителят е вече записан за това събитие!" });

        if (_context.Submits.Where(e => e.EventId == eventId).Count() >= _context.Events.Find(eventId)!.PeopleLimit)
            return Results.BadRequest(new { error = "Няма свободни места!" });

        if (_context.Events.Find(eventId).SignUpDeadline < DateTime.UtcNow)
            return Results.BadRequest(new { error = "Срокът за записване е изтекъл!"});

        var entity = SubmitMapper.ToEntity(eventId, userId, dto);
        _context.Submits.Add(entity);
        return _context.SaveChanges() > 0 ? Results.Created($"/submits/{eventId}", dto) : Results.InternalServerError("Failed to save submit");
    }

    public IResult UpdateSubmission(int eventId, string userId, UpdateSubmitDto dto)
    {
        var submit = _context.Submits
            .Include(s => s.Event)
            .FirstOrDefault(s => s.EventId == eventId && s.UserId == userId);

        if (submit == null)
            return Results.BadRequest(new { error = "Не е намерена заявка"});

        if (submit.Event.SignUpDeadline < DateTime.UtcNow)
            return Results.BadRequest(new { error = "Срокът за записване е изтекъл!" });

        SubmitMapper.UpdateEntity(submit, dto);
        submit.Date = DateTime.UtcNow;
        submit.UpdatedAt = DateTime.UtcNow;

        _context.Submits.Update(submit);
        return _context.SaveChanges() > 0 ? Results.Created($"/submits/{submit.EventId}", new { submit }) : Results.InternalServerError(new { error = "Oбновяването на заявката мина неуспешно." });
    }


    // Admin removes user from an Event
    public async Task<bool> AdminRemoveUserFromEvent(int eventId, string userId, IEmailSender _emailSender)
    {
        // Fetch submission with related user and event data
        var submission = await _context.Submits
            .Include(s => s.User)
            .Include(s => s.Event)
            .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == userId);

        if (submission?.User == null || submission.Event == null)
            return false;

        // Prepare email details
        var userEmail = submission.User.Email;
        var userName = userEmail.Contains('@')
            ? userEmail[..userEmail.IndexOf('@')]
            : "User";

        var eventName = submission.Event.Name;
        var eventDate = submission.Event.Date?.ToString("MMMM d, yyyy") ?? "бъдеща дата";

        // Remove the submission
        _context.Submits.Remove(submission);
        var success = await _context.SaveChangesAsync() > 0;

        // Send email only if removal succeeded
        if (success)
        {
            await _emailSender.SendEmailAsync(
            email: userEmail,
            subject: $"Премахнат от събитието: {eventName}",
            htmlMessage: $@"
                <html>
                    <body>
                        <p>Уважаеми/а {userName},</p>
                        <p>Администратор ви е премахнал от събитието <strong>{eventName}</strong>, насрочено за {eventDate}.</p>
                        <p>Ако имате въпроси, моля свържете се с администратор.</p>                           
                    </body>
                </html>"
);

        }

        return success;
    }
    //User removes himself from the event
    public async Task<IResult> RemoveUserFromEvent(int eventId, string userId, IEmailSender _emailSender)
    {
        // Fetch submission with related user and event data
        var submission = await _context.Submits
            .Include(s => s.User)
            .Include(s => s.Event)
            .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == userId);

        if (submission == null)
            return Results.BadRequest();

        if (submission?.User == null || submission.Event == null)
            return Results.InternalServerError();

        if (_context.Events.Find(eventId).SignUpDeadline < DateTime.UtcNow)
            return Results.BadRequest(new { error = "Срокът за отписване е изтекъл!" });

        // Prepare email details
        var userEmail = submission.User.Email;
        var userName = userEmail.Contains('@')
            ? userEmail[..userEmail.IndexOf('@')]
            : "User";

        var eventName = submission.Event.Name;
        var eventDate = submission.Event.Date?.ToString("MMMM d, yyyy") ?? "бъдеща дата";

        // Remove the submission
        _context.Submits.Remove(submission);
        var success = await _context.SaveChangesAsync() > 0;

        // Send email only if removal succeeded
        if (success)
        {
            await _emailSender.SendEmailAsync(
                email: userEmail,
                subject: $"Премахнат от събитието: {eventName}",
                htmlMessage: $@"
                    <html>
                        <body>
                            <p>Уважаеми/а {userName},</p>
                            <p>Вие успешно се отписахте от събитието <strong>{eventName}</strong>, насрочено за {eventDate}.</p>
                            <p>Ако това е било грешка, можете да се запишете отново по всяко време преди крайния срок за регистрация.</p>
                        </body>
                    </html>"
            );
        }

        return success ? Results.Ok() : Results.InternalServerError();
    }


}

