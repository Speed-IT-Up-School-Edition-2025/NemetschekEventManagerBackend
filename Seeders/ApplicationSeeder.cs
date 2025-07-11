using Microsoft.AspNetCore.Identity;
using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Seeders
{
	public class ApplicationSeeder : IHostedService
	{

		private readonly IServiceProvider _serviceProvider;

		public ApplicationSeeder(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<EventDbContext>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

			await SeederLogic.SeedAsync(context, userManager); // Move your existing seeding logic to this static class
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	}
}
