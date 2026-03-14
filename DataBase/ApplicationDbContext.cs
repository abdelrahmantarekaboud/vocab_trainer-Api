using VocabTrainer.Api.Entities;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Api.Authentication;

namespace VocabTrainer.Api.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : base(opt)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Language> Languages => Set<Language>();
        public DbSet<Word> Words => Set<Word>();
        public DbSet<InvitationCode> InvitationCodes => Set<InvitationCode>();
        public DbSet<InvitationRedemption> InvitationRedemptions => Set<InvitationRedemption>();
        public DbSet<QuizSession> QuizSessions => Set<QuizSession>();
        public DbSet<QuizItemResult> QuizItemResults => Set<QuizItemResult>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<UserUpdateLog> UserUpdateLogs => Set<UserUpdateLog>();
        public DbSet<UserLanguageStats> UserLanguageStats { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(b);
        }
    }
}