using VocabTrainer.Api.DataBase;
using Microsoft.EntityFrameworkCore;

namespace VocabTrainer.Api.Services.Background
{
    public class GuestCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public GuestCleanupBackgroundService(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var cutoff = DateTime.UtcNow.AddDays(-7);
                var guests = await db.Users
                    .Where(u => u.IsGuest && u.CreatedAt <= cutoff)
                    .ToListAsync(stoppingToken);

                if (guests.Count > 0)
                {
                    db.Users.RemoveRange(guests);
                    await db.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}