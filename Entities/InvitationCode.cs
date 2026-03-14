using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Entities
{
    public class InvitationCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public CodeType Type { get; set; }

        public UserRole TargetRole { get; set; }
        public List<string> Languages { get; set; } = new();

        public int MaxUses { get; set; } = 1;
        public int UsedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime? ExpiresAt { get; set; }

        public Guid CreatedByAdminId { get; set; }
        public Guid? TargetUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}