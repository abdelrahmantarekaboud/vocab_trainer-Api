using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Users;

namespace VocabTrainer.Api.Services.Account
{
    public interface IAccountService
    {
        Task<Result<UserDto>> GetProfile(Guid userId);

        Task<Result<UserDto>> UpdateSettings(Guid userId, UpdateSettingsRequest req);
    }
}