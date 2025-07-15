using Microsoft.AspNetCore.Identity;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;

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
            // Seed Users if none exist
            if (_context.Users.Count() <= 1)
            {
                var passwordHasher = new PasswordHasher<User>();

                List<User> users = new List<User>
                {
                new User
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
                },

                new User
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
                },
                new User
                {
                    UserName = "georgi.todorov@example.com",
                    NormalizedUserName = "GEORGI.TODOROV@EXAMPLE.COM",
                    Email = "georgi.todorov@example.com",
                    NormalizedEmail = "GEORGI.TODOROV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "elena.georgieva@example.com",
                    NormalizedUserName = "ELENA.GEORGIEVA@EXAMPLE.COM",
                    Email = "elena.georgieva@example.com",
                    NormalizedEmail = "ELENA.GEORGIEVA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "nikolay.iliev@example.com",
                    NormalizedUserName = "NIKOLAY.ILIEV@EXAMPLE.COM",
                    Email = "nikolay.iliev@example.com",
                    NormalizedEmail = "NIKOLAY.ILIEV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "kristina.petrova@example.com",
                    NormalizedUserName = "KRISTINA.PETROVA@EXAMPLE.COM",
                    Email = "kristina.petrova@example.com",
                    NormalizedEmail = "KRISTINA.PETROVA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "stefan.markov@example.com",
                    NormalizedUserName = "STEFAN.MARKOV@EXAMPLE.COM",
                    Email = "stefan.markov@example.com",
                    NormalizedEmail = "STEFAN.MARKOV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "vesela.angelova@example.com",
                    NormalizedUserName = "VESELA.ANGELOVA@EXAMPLE.COM",
                    Email = "vesela.angelova@example.com",
                    NormalizedEmail = "VESELA.ANGELOVA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "petar.stoyanov@example.com",
                    NormalizedUserName = "PETAR.STOYANOV@EXAMPLE.COM",
                    Email = "petar.stoyanov@example.com",
                    NormalizedEmail = "PETAR.STOYANOV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "daniela.ivanova@example.com",
                    NormalizedUserName = "DANIELA.IVANOVA@EXAMPLE.COM",
                    Email = "daniela.ivanova@example.com",
                    NormalizedEmail = "DANIELA.IVANOVA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "viktor.dimitrov@example.com",
                    NormalizedUserName = "VIKTOR.DIMITROV@EXAMPLE.COM",
                    Email = "viktor.dimitrov@example.com",
                    NormalizedEmail = "VIKTOR.DIMITROV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "snejana.koleva@example.com",
                    NormalizedUserName = "SNEJANA.KOLEVA@EXAMPLE.COM",
                    Email = "snejana.koleva@example.com",
                    NormalizedEmail = "SNEJANA.KOLEVA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "kalin.mihaylov@example.com",
                    NormalizedUserName = "KALIN.MIHAYLOV@EXAMPLE.COM",
                    Email = "kalin.mihaylov@example.com",
                    NormalizedEmail = "KALIN.MIHAYLOV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "irina.vasileva@example.com",
                    NormalizedUserName = "IRINA.VASILEVA@EXAMPLE.COM",
                    Email = "irina.vasileva@example.com",
                    NormalizedEmail = "IRINA.VASILEVA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                new User
                {
                    UserName = "martin.ivanov@example.com",
                    NormalizedUserName = "MARTIN.IVANOV@EXAMPLE.COM",
                    Email = "martin.ivanov@example.com",
                    NormalizedEmail = "MARTIN.IVANOV@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = passwordHasher.HashPassword(null, "Test123!"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
                };
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }

            // Seed Events if none exist
            if (!_context.Events.Any())
            {
                List<Event> events = new List<Event>
                {
                    new Event
                    {
                        Name = "Лятно фирмено парти",
                        Description = "Събиране на служителите в неформална обстановка с музика и игри.",
                        Date = new DateTime(2025, 8, 5, 18, 0, 0),
                        SignUpDeadline = new DateTime(2025, 7, 20),
                        Location = "Витоша, хижа Алеко",
                        PeopleLimit = null,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "text",
                            Name = "Отедел",
                            Required = true,
                            Options = new List<string>()
                        },
                        new Field
                        {
                            Id = 2,
                            Type = "radio",
                            Name = "Желаете ли транспорт от София?",
                            Required = false,
                            Options = new List<string> { "Да", "Не" }
                        },
                        new Field
                        {
                            Id = 3,
                            Type = "radio",
                            Name = "Ще участвате ли в игрите?",
                            Required = false,
                            Options = new List<string> { "Да", "Не" }
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Семинар по киберсигурност",
                        Description = "Вътрешно обучение по основи на ИТ сигурността и защита на данни.",
                        Date = new DateTime(2025, 9, 15, 9, 0, 0),
                        SignUpDeadline = new DateTime(2025, 8, 31),
                        Location = "София, Зала 2 - Бизнес Център",
                        PeopleLimit = 25,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "radio",
                            Name = "Ниво на познания в киберсигурността",
                            Required = true,
                            Options = new List<string>
                            {
                                "Начинаещ",
                                "Средно ниво",
                                "Напреднал"
                            },
                        },
                        new Field
                        {
                            Id = 2,
                            Type = "radio",
                            Name = "Желаете ли да получите сертификат?",
                            Required = false,
                            Options = new List<string> { "Да", "Не" }
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Hackathon 2025",
                        Description = "24-часов хакатон за разработка на иновативни приложения.",
                        Date = new DateTime(2025, 10, 12, 10, 0, 0),
                        SignUpDeadline = new DateTime(2025, 10, 1),
                        Location = "София Тех Парк",
                        PeopleLimit = 30,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "text",
                            Name = "Технологии, с които ще работите",
                            Required = true,
                            Options = new List<string>()
                        },
                        new Field
                        {
                            Id = 2,
                            Type = "radio",
                            Name = "Имате ли екип?",
                            Required = false,
                            Options = new List<string> { "Да", "Не" }
                        },
                        new Field
                        {
                            Id = 3,
                            Type = "text",
                            Name = "Име на екипа (ако имате)",
                            Required = true,
                            Options = new List<string>()
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Уикенд тиймбилдинг",
                        Description = "Двудневно излизане извън града за сплотяване на екипа.",
                        Date = new DateTime(2025, 5, 25, 10, 0, 0),
                        SignUpDeadline = new DateTime(2025, 5, 10),
                        Location = "Сапарева баня",
                        PeopleLimit = 30,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "radio",
                            Name = "Нужда от транспорт",
                            Required = false,
                            Options = new List<string> { "Да", "Не" }
                        },
                        new Field
                        {
                            Id = 2,
                            Type = "text",
                            Name = "Предпочитание за настаняване",
                            Required = false,
                            Options = new List<string>()
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Пролетен уъркшоп: UX дизайн",
                        Description = "Интензивна сесия за основи на потребителския интерфейс и преживяване.",
                        Date = new DateTime(2025, 4, 10, 14, 0, 0),
                        SignUpDeadline = new DateTime(2025, 3, 31),
                        Location = "Онлайн",
                        PeopleLimit = 20,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "radio",
                            Name = "Ниво на опит",
                            Required = true,
                            Options = new List<string>
                            {
                                "Начинаещ",
                                "Средно ниво",
                                "Напреднал"
                            }
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Мини кодинг буткамп",
                        Description = "Интензивен курс по програмиране за начинаещи.",
                        Date = new DateTime(2025, 8, 22, 9, 0, 0),
                        SignUpDeadline = new DateTime(2025, 8, 10),
                        Location = "Варна, офис пространство",
                        PeopleLimit = 10,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "radio",
                            Name = "Имате ли собствен лаптоп?",
                            Required = true,
                            Options = new List<string> { "Да", "Не" }
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Група за обратна връзка на продукт",
                        Description = "Обсъждане и събиране на идеи за подобрение на новия вътрешен софтуер.",
                        Date = new DateTime(2025, 8, 30, 16, 30, 0),
                        SignUpDeadline = new DateTime(2025, 8, 25),
                        Location = "София, етаж 4 - Конферентна зала",
                        PeopleLimit = 10,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "text",
                            Name = "Какъв е вашият опит със софтуера?",
                            Required = true,
                            Options = new List<string>()
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Есенни технологични лекции",
                        Description = "Серия от лекции по актуални технологии в индустрията.",
                        Date = new DateTime(2025, 10, 3, 13, 0, 0),
                        SignUpDeadline = new DateTime(2025, 9, 25),
                        Location = "Пловдив, Конферентен център",
                        PeopleLimit = 25,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "checkbox",
                            Name = "Интересуваща тема",
                            Required = true,
                            Options = new List<string>
                            {
                                "AI",
                                "Cloud",
                                "DevOps"
                            }
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Event
                    {
                        Name = "Ден на благосъстоянието",
                        Description = "Активности и лекции, насочени към ментално и физическо здраве.",
                        Date = new DateTime(2025, 9, 10, 10, 0, 0),
                        SignUpDeadline = new DateTime(2025, 8, 31),
                        Location = "София, парк Борисова градина",
                        PeopleLimit = 40,
                        Fields = new List<Field>
                    {
                        new Field
                        {
                            Id = 1,
                            Type = "radio",
                            Name = "Желаете ли да участвате в йога?",
                            Required = false,
                            Options = new List<string> { "Да", "Не" }
                        },
                        new Field
                        {
                            Id = 2,
                            Type = "radio",
                            Name = "Имате ли хранителни ограничения?",
                            Required = false,
                            Options = new List<string> { "Да", "Не" }
                        }
                    },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await _context.Events.AddRangeAsync(events);
                await _context.SaveChangesAsync();
            }

            // Seed Submits if none exist
            if (!_context.Submits.Any())
            {
                if (!_context.Submits.Any(s => s.EventId == 1))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(12)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "admin@example.com"));

                    var departments = new[] { "ИТ отдел", "Маркетинг", "Финанси", "Продажби", "Човешки ресурси" };
                    var yesNo = new[] { "Да", "Не" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Отдел",
                                Options = new List<string> { departments[rnd.Next(departments.Length)] }
                            },
                            new Submission
                            {
                                Id = 2,
                                Name = "Желаете ли транспорт от София?",
                                Options = new List<string> { yesNo[rnd.Next(yesNo.Length)] }
                            },
                            new Submission
                            {
                                Id = 3,
                                Name = "Ще участвате ли в игрите?",
                                Options = new List<string> { yesNo[rnd.Next(yesNo.Length)] }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 1,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 2))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Reverse()
                        .Take(12)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "ivan.petrov@example.com"));

                    var skill = new[] { "Начинаещ", "Средно ниво", "Напреднал" };
                    var yesNo = new[] { "Да", "Не" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Ниво на познания в киберсигурността",
                                Options = new List<string> { skill[rnd.Next(skill.Length)] }
                            },
                            new Submission
                            {
                                Id = 2,
                                Name = "Желаете ли да получите сертификат?",
                                Options = new List<string> { yesNo[rnd.Next(yesNo.Length)] }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 2,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 3))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Reverse()
                        .Take(12)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "ivan.petrov@example.com"));

                    var technologies = new[]
                    {
                        "C#, Blazor, SQL Server",
                        "Python, Flask, PostgreSQL",
                        "JavaScript, React, Firebase",
                        "Java, Spring Boot, MySQL",
                        "Node.js, Vue, MongoDB"
                    };

                    var teamNames = new[]
                    {
                        "CodeStorm", "BugSlayers", "DevWizards", "ByteForce", "Hacktastic"
                    };

                    var yesNo = new[] { "Да", "Не" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var hasTeam = yesNo[rnd.Next(2)];
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Технологии, с които ще работите",
                                Options = new List<string> { technologies[rnd.Next(technologies.Length)] }
                            },
                            new Submission
                            {
                                Id = 2,
                                Name = "Имате ли екип?",
                                Options = new List<string> { hasTeam }
                            },
                            new Submission
                            {
                                Id = 3,
                                Name = "Име на екипа (ако имате)",
                                Options = hasTeam == "Да"
                                    ? new List<string> { teamNames[rnd.Next(teamNames.Length)] }
                                    : new List<string> { "" }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 3,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 4))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Reverse()
                        .Take(10)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "ivan.petrov@example.com"));

                    var accommodationPrefs = new[]
                    {
                        "Единична стая",
                        "Двойна стая с колега",
                        "Няма предпочитания"
                    };

                    var yesNo = new[] { "Да", "Не" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Нужда от транспорт",
                                Options = new List<string> { yesNo[rnd.Next(2)] }
                            },
                            new Submission
                            {
                                Id = 2,
                                Name = "Предпочитание за настаняване",
                                Options = new List<string> { accommodationPrefs[rnd.Next(accommodationPrefs.Length)] }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 4,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 5))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Reverse()
                        .Take(14)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "admin@example.com"));

                    var accommodationPrefs = new[]
                    {
                        "Единична стая",
                        "Двойна стая с колега",
                        "Няма предпочитания"
                    };

                    var experienceLevels = new[] { "Начинаещ", "Средно ниво", "Напреднал" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Ниво на опит",
                                Options = new List<string> { experienceLevels[rnd.Next(experienceLevels.Length)] }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 5,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 6))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Reverse()
                        .Take(9)
                        .ToList();

                    var yesNo = new[] { "Да", "Не" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Имате ли собствен лаптоп?",
                                Options = new List<string> { yesNo[rnd.Next(2)] }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 6,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 7))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(9)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "admin@example.com"));

                    var experienceOptions = new[]
                    {
                        "Използвам го редовно за отчитане",
                        "Работя с него отскоро",
                        "Само съм преглеждал функционалностите",
                        "Имам предложения за подобрение",
                        "Доволен съм от интерфейса",
                        "Трудно ми е да намирам определени функции"
                    };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Какъв е вашият опит със софтуера?",
                                Options = new List<string> { experienceOptions[rnd.Next(experienceOptions.Length)] }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 7,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 8))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(14)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "admin@example.com"));

                    var topics = new[] { "AI", "Cloud", "DevOps" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                                Id = 1,
                                Name = "Интересуваща тема",
                                Options = new List<string> { topics[rnd.Next(topics.Length)] }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 8,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }

                if (!_context.Submits.Any(s => s.EventId == 9))
                {
                    var users = _context.Users
                        .Where(u => u.Email != "ivan.petrov@example.com" && u.Email != "admin@example.com")
                        .OrderBy(_ => Guid.NewGuid())
                        .Reverse()
                        .Take(14)
                        .ToList();

                    users.Add(_context.Users.First(u => u.Email == "ivan.petrov@example.com"));
                    users.Add(_context.Users.First(u => u.Email == "admin@example.com"));

                    var yesNo = new[] { "Да", "Не" };
                    var rnd = new Random();

                    var submits = new List<Submit>();

                    foreach (var user in users)
                    {
                        var submissions = new List<Submission>
                        {
                            new Submission
                            {
                            Id = 1,
                            Name = "Желаете ли да участвате в йога?",
                            Options = new List<string> { rnd.Next(2) == 0 ? "Да" : "Не" }
                            },
                            new Submission
                            {
                                Id = 2,
                                Name = "Имате ли хранителни ограничения?",
                                Options = new List<string> { rnd.Next(2) == 0 ? "Да" : "Не" }
                            }
                        };

                        var submit = new Submit
                        {
                            UserId = user.Id,
                            EventId = 9,
                            Date = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Submissions = submissions
                        };

                        submits.Add(submit);
                    }

                    _context.Submits.AddRange(submits);
                    _context.SaveChanges();
                }
            }
        }
    }
}
