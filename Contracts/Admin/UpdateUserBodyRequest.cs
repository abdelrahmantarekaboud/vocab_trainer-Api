namespace VocabTrainer.Api.Contracts.Admin
{
    public record UpdateUserBodyRequest(Guid Id, AdminUpdateUserRequest Data);
}