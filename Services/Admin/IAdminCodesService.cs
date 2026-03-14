using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Admin;

namespace VocabTrainer.Api.Services.Admin
{
    public interface IAdminCodesService
    {
        Task<Result<List<CodeDto>>> GenerateInviteCodes(Guid adminId, GenerateInviteCodesRequest req);

        Task<Result<List<CodeDto>>> GenerateLanguageCodes(Guid adminId, GenerateLanguageCodesRequest req);

        Task<Result<AdminCodesResponse>> ListCodesGrouped();
        Task<Result> Disable(Guid id);
        Task<Result> Delete(Guid id);

    }
}