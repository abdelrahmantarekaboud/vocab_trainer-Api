using VocabTrainer.Api.Entities;

namespace VocabTrainer.Api.Authentication
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(User user);

        RefreshToken GenerateRefreshToken(User user);
    }
}