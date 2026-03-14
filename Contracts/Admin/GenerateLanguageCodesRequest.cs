namespace VocabTrainer.Api.Contracts.Admin
{
    public record GenerateLanguageCodesRequest(
        List<string> Languages,
        int Count,
        DateTime? ExpiresAt,
        Guid? TargetUserId
    );
}