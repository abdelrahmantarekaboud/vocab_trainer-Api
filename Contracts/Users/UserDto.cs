namespace VocabTrainer.Api.Contracts.Users
{
    public record UserDto(
        Guid Id,
        string Name,
        string Email,
        string Role,
        bool IsGuest,
        List<string> AllowedLanguages,
        string CurrentLanguageId,
        string UiLanguage,
        string Theme,
        double TtsSpeed,
        int TtsRepeatCount,
        string? TtsVoiceId
    );
}