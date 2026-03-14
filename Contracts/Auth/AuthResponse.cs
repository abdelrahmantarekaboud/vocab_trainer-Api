using VocabTrainer.Api.Contracts.Users;

namespace VocabTrainer.Api.Contracts.Auth
{
    public record AuthResponse(string AccessToken, string RefreshToken, UserDto User);
}