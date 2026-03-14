namespace VocabTrainer.Api.Contracts.Admin
{
    public record CodeDto(
        Guid Id,
        string Code,
        string Type,
        string TargetRole,
        List<string> Languages,
        bool IsActive,
        int RemainingUses,
        DateTime? ExpiresAt,
        DateTime CreatedAt
    );
}