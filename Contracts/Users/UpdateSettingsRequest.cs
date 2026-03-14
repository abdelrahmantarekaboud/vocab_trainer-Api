namespace VocabTrainer.Api.Contracts.Users
{
    public record UpdateSettingsRequest(
        string? UiLanguage,
        string? Theme,
        double? TtsSpeed,
        int? TtsRepeatCount,
        string? TtsVoiceId,
        string? CurrentLanguageId
    );
}