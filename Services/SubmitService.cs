using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using System;


namespace NemetschekEventManagerBackend.Services
{
    public class SubmitService : ISubmitService
    {
        private readonly EventDbContext _context;

        public SubmitService(EventDbContext context)
        {
            _context = context;
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
}
