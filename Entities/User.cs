using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;

        public UserRole Role { get; set; } = UserRole.User;
        public bool IsGuest { get; set; }
        public bool EmailConfirmed { get; set; } = true;

        public List<string> AllowedLanguages { get; set; } = new();
        public string CurrentLanguageId { get; set; } = "en";

        public string UiLanguage { get; set; } = "ar";
        public string Theme { get; set; } = "dark";

        public double TtsSpeed { get; set; } = 1.0;
        public int TtsRepeatCount { get; set; } = 3;
        public string? TtsVoiceId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}