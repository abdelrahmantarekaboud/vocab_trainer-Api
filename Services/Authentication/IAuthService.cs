using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Auth;

namespace VocabTrainer.Api.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> Login(LoginRequest req);

        Task<Result<AuthResponse>> RegisterWithCode(RegisterWithCodeRequest req);

        Task<Result<AuthResponse>> Guest();

        Task<Result<AuthResponse>> Refresh(RefreshRequest req);
    }
}