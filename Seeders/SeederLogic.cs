using Microsoft.AspNetCore.Identity;
using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Seeders
{
	public class SeederLogic
	{
		public static async Task SeedAsync(EventDbContext context, UserManager<User> userManager)
		{
			// Seed Users
			if (!userManager.Users.Any())
			{
				List<User> user = new List<User>()
				{
					new User
					{
						UserName = "john.doe@example.com",
						Email = "john.doe@example.com",
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new User
					{
						UserName = "MEGAKNIGHT@example.com",
						Email = "MEGAKNIGHT@example.com",
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new User
					{
						UserName = "Person@example.com",
						Email = "Pearson@example.com",
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					}
				};

				for (int i = 0; i < user.Count; i++)
				{
					await userManager.CreateAsync(user[i], "Password123!");
				}
			}

			// Seed Events
			if (!context.Events.Any())
			{
				var event1 = new List<Event>
				{
					new Event
					{
						Name = "Хакатон 2025",
						Description = "Годишно състезание по програмиране.",
						Date = DateTime.UtcNow.AddMonths(1),
						SignUpDeadline = DateTime.UtcNow.AddDays(15),
						Location = "Онлайн",
						PeopleLimit = 10,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new Event{
						Name = "Технологична конференция 2025",
						Description = "Годишна технологична конференция.",
						Date = DateTime.UtcNow.AddMonths(2),
						SignUpDeadline = DateTime.UtcNow.AddDays(30),
						Location = "София",
						PeopleLimit = 500,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new Event
					{
						Name = "AI Среща 2025",
						Description = "Среща за последните постижения в изкуствения интелект.",
						Date = DateTime.UtcNow.AddMonths(3),
						SignUpDeadline = DateTime.UtcNow.AddDays(45),
						Location = "Пловдив",
						PeopleLimit = 30,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new Event
					{
						Name = "Кариерен форум 2025",
						Description = "Събитие за намиране на работа и професионално развитие.",
						Date = DateTime.UtcNow.AddMonths(4),
						SignUpDeadline = DateTime.UtcNow.AddDays(60),
						Location = "Варна",
						PeopleLimit = 25,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new Event
					{
						Name = "Фестивал на роботиката 2025",
						Description = "Презентации и състезания с роботи.",
						Date = DateTime.UtcNow.AddMonths(5),
						SignUpDeadline = DateTime.UtcNow.AddDays(75),
						Location = "Бургас",
						PeopleLimit = 15,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					}
				};

				for (int i = 0; i < event1.Count; i++)
				{
					context.Events.Add(event1[i]);
					await context.SaveChangesAsync();
				}
			}

			// Seed Submits
			if (!context.Submits.Any())
			{
				var user = await userManager.FindByEmailAsync("john.doe@example.com");
				var ev = context.Events.First();

				if (user is null)
				{
					user = new User
					{
						UserName = "john.doe@example.com",
						Email = "john.doe@example.com",
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					};

					await userManager.CreateAsync(user, "Password123!");
				}

				var submit = new List<Submit>
				{
					new Submit
					{
						UserId = user.Id,
						EventId = ev.Id,
						Date = DateTime.UtcNow,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new Submit
					{
						UserId = user.Id + 1,
						EventId = ev.Id + 1,
						Date = DateTime.UtcNow.AddDays(10),
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					},
					new Submit
					{
						UserId = user.Id + 2,
						EventId = ev.Id + 2,
						Date = DateTime.UtcNow.AddDays(30),
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					}
				};

				for (int i = 0; i < submit.Count; i++)
				{
					context.Submits.Add(submit[i]);
					await context.SaveChangesAsync();
				}
			}
		}

	}
}
