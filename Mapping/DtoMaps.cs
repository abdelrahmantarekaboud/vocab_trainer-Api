using VocabTrainer.Api.Entities;
using VocabTrainer.Api.Contracts.Admin;
using VocabTrainer.Api.Contracts.Users;
using VocabTrainer.Api.Contracts.Words;
using VocabTrainer.Api.Contracts.Languages;

namespace VocabTrainer.Api.Mapping
{
    public static class DtoMaps
    {
        public static UserDto ToDto(this User u) =>
            new(u.Id, u.Name, u.Email, u.Role.ToString(), u.IsGuest,
                u.AllowedLanguages, u.CurrentLanguageId, u.UiLanguage, u.Theme,
                u.TtsSpeed, u.TtsRepeatCount, u.TtsVoiceId);

        public static LanguageDto ToDto(this Language l) =>
            new(l.Id, l.NameEn, l.NameAr, l.Locale, l.IsActive);

        public static CodeDto ToDto(this InvitationCode c) =>
            new(c.Id, c.Code, c.Type.ToString(), c.TargetRole.ToString(), c.Languages,
                c.IsActive, c.MaxUses - c.UsedCount, c.ExpiresAt, c.CreatedAt);
        public static AdminUserDto ToAdminDto(this User u) => new(
    u.Id,
    u.Name,
    u.Email,
    u.Role.ToString(),
    u.IsGuest,
    u.EmailConfirmed,
    u.AllowedLanguages,
    u.CurrentLanguageId,
    u.UiLanguage,
    u.Theme,
    u.TtsSpeed,
    u.TtsRepeatCount,
    u.TtsVoiceId,
    u.CreatedAt
);


        public static WordDto ToDto(this Word w) =>
  new(
  w.Id,
  w.LanguageId,
  w.Text,
  w.Translation,
  w.Example,
  w.Topic,
  w.RecordingUrl,
  w.ReviewCount,
  w.CorrectCount,
  w.WrongCount,
  w.IntervalDays,
  w.EaseFactor,
  w.NextReviewAt
);
    }
}