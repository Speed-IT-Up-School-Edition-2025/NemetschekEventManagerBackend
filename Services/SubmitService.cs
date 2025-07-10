using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;
using System.Threading.Tasks;

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
            .Where(s => s.EventId == eventId)
            .Select(s => SubmitMapper.ToSummaryDto(s))
            .ToList();
    }

    public Submit? GetSubmitByEventAndUser(int eventId, string userId)
    {
        return _context.Submits
            .FirstOrDefault(s => s.EventId == eventId && s.UserId == userId);
    }

    public bool Create(int eventId, string userId, CreateSubmitDto dto)
    {
        if (_context.Submits.Any(s => s.EventId == eventId && s.UserId == userId))
            return false;

        if (_context.Submits.Where(e => e.EventId == eventId).Count() >= _context.Events.Find(eventId)!.PeopleLimit)
            return false;

        var entity = SubmitMapper.ToEntity(eventId, userId, dto);
        _context.Submits.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool UpdateSubmission(int eventId, string userId, UpdateSubmitDto dto)
    {
        var submit = GetSubmitByEventAndUser(eventId, userId);
        if (submit == null)
            return false;

        SubmitMapper.UpdateEntity(submit, dto);
        submit.Date = DateTime.UtcNow;
        submit.UpdatedAt = DateTime.UtcNow;

        _context.Submits.Update(submit);
        return _context.SaveChanges() > 0;
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
        var eventDate = submission.Event.Date?.ToString("MMMM d, yyyy") ?? "a future date";

        // Remove the submission
        _context.Submits.Remove(submission);
        var success = await _context.SaveChangesAsync() > 0;

        // Send email only if removal succeeded
        if (success)
        {   
            await _emailSender.SendEmailAsync(
                email: userEmail,
                subject: $"Removed from Event: {eventName}",
                htmlMessage: $@"
                    <html>
                        <body>
                            <p>Dear {userName},</p>
                            <p>An administrator has removed you from the event <strong>{eventName}</strong> scheduled for {eventDate}.</p>
                            <p>If you have any questions, please contact an administrator.</p>                           
                        </body>
                    </html>"
            );
        }

        return success;
    }
    //User removes himself from the event
    public async Task<bool> RemoveUserFromEvent(int eventId, string userId, IEmailSender _emailSender)
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
        var eventDate = submission.Event.Date?.ToString("MMMM d, yyyy") ?? "a future date";

        // Remove the submission
        _context.Submits.Remove(submission);
        var success = await _context.SaveChangesAsync() > 0;

        // Send email only if removal succeeded
        if (success)
        {
            await _emailSender.SendEmailAsync(
                email: userEmail,
                subject: $"Removed from Event: {eventName}",
                htmlMessage: $@"
                    <html>
                        <body>
                       <p>Dear {userName},</p>
                       <p>You have successfully unsubmitted from the event <strong>{eventName}</strong> scheduled for {eventDate}.</p>
                      <p>If this was a mistake, you can re-submit at any time before the sign-up deadline.</p>
                        </body>
                    </html>"
            );
        }

        return success;
    }


}

