using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Admin;
using VocabTrainer.Api.Contracts.Languages;

namespace VocabTrainer.Api.Services.Admin
{
    public interface IAdminLanguagesService
    {
        Task<Result<List<LanguageDto>>> ListAll();

        Task<Result> Toggle(string id, bool active);
        Task<Result<LanguageDto>> AddLanguageByName(AddLanguageByNameRequest request);
        Task<Result> Delete(string id);

    }
}