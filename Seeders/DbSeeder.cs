﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Seeders
{
    public class DbSeeder
    {
        private readonly EventDbContext _context;

        public DbSeeder(EventDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if(_context.Users.Any() || _context.Events.Any() || _context.Submits.Any())
                return;

            await _context.Database.EnsureCreatedAsync();

            // Create users
            var passwordHasher = new PasswordHasher<User>();

            var user1 = new User
            {
                UserName = "ivan.petrov@example.com",
                NormalizedUserName = "IVAN.PETROV@EXAMPLE.COM",
                Email = "ivan.petrov@example.com",
                NormalizedEmail = "IVAN.PETROV@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var user2 = new User
            {
                UserName = "maria.ivanova@example.com",
                NormalizedUserName = "MARIA.IVANOVA@EXAMPLE.COM",
                Email = "maria.ivanova@example.com",
                NormalizedEmail = "MARIA.IVANOVA@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();

            // Create events
            var event1 = new Event
            {
                Name = "Лятно фирмено парти",
                Description = "Събиране на служителите в неформална обстановка с музика и игри.",
                Date = new DateTime(2025, 8, 5, 18, 0, 0),
                SignUpDeadline = new DateTime(2025, 7, 20),
                Location = "Витоша, хижа Алеко",
                PeopleLimit = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var event2 = new Event
            {
                Name = "Семинар по киберсигурност",
                Description = "Вътрешно обучение по основи на ИТ сигурността и защита на данни.",
                Date = new DateTime(2025, 9, 15, 9, 0, 0),
                SignUpDeadline = new DateTime(2025, 8, 31),
                Location = "София, Зала 2 - Бизнес Център",
                PeopleLimit = 50,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var event3 = new Event
            {
                Name = "Hackathon 2025",
                Description = "24-часов хакатон за разработка на иновативни приложения.",
                Date = new DateTime(2025, 10, 12, 10, 0, 0),
                SignUpDeadline = new DateTime(2025, 10, 1),
                Location = "София Тех Парк",
                PeopleLimit = 60,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Events.AddRangeAsync(event1, event2, event3);
            await _context.SaveChangesAsync();

            // Submits
            var submits = new List<Submit>
            {
                new Submit
                {
                    UserId = user1.Id,
                    EventId = event1.Id,
                    Date = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Submit
                {
                    UserId = user1.Id,
                    EventId = event2.Id,
                    Date = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Submit
                {
                    UserId = user2.Id,
                    EventId = event2.Id,
                    Date = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await _context.Submits.AddRangeAsync(submits);
            await _context.SaveChangesAsync();
        }
    }
}
