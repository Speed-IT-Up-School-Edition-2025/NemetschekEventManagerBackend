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
    
    public bool RemoveUserFromEvent(int eventId, string userId)
    {
        var submission = _context.Submits
            .FirstOrDefault(s => s.EventId == eventId && s.UserId == userId);

        if (submission == null)
                return false;

        _context.Submits.Remove(submission);
        return _context.SaveChanges() != 0;
    }
}
