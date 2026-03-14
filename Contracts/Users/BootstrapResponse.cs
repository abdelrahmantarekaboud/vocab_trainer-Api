using VocabTrainer.Api.Contracts.Languages;

namespace VocabTrainer.Api.Contracts.Users
{
    public record BootstrapResponse(UserDto User, List<LanguageDto> AllowedLanguages);
}