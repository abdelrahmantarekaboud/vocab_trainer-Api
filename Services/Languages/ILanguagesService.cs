using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Languages;

namespace VocabTrainer.Api.Services.Languages
{
    public interface ILanguagesService
    {
        Task<Result<List<LanguageDto>>> GetAllowed(Guid userId);

        Task<Result<List<LanguageDto>>> GetAllActive();
        Task<Result<List<LanguageDto>>> RedeemLanguageCode(Guid userId, RedeemLanguageCodeRequest req); // ✅ new


    }
}