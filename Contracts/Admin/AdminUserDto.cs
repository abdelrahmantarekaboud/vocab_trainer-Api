namespace VocabTrainer.Api.Contracts.Admin;

public record AdminUserDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    bool IsGuest,
    bool EmailConfirmed,
    List<string> AllowedLanguages,
    string CurrentLanguageId,
    string UiLanguage,
    string Theme,
    double TtsSpeed,
    int TtsRepeatCount,
    string? TtsVoiceId,
    DateTime CreatedAt
);