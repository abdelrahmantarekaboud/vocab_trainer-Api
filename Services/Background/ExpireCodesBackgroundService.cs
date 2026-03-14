using VocabTrainer.Api.DataBase;
using Microsoft.EntityFrameworkCore;

namespace VocabTrainer.Api.Services.Background
{
    public class ExpireCodesBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ExpireCodesBackgroundService(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;
                var expired = await db.InvitationCodes
                    .Where(c => c.IsActive && c.ExpiresAt != null && c.ExpiresAt <= now)
                    .ToListAsync(stoppingToken);

                foreach (var c in expired) c.IsActive = false;
                if (expired.Count > 0) await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}