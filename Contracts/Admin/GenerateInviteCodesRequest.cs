using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Contracts.Admin
{
    public record GenerateInviteCodesRequest(
        UserRole TargetRole,
        List<string> Languages,
        int Count,
        DateTime? ExpiresAt
    );
}