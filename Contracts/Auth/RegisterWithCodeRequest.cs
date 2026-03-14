namespace VocabTrainer.Api.Contracts.Auth
{
    public record RegisterWithCodeRequest(string Code, string Name, string Email, string Password);
}