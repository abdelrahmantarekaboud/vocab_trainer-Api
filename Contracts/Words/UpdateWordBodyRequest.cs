namespace VocabTrainer.Api.Contracts.Words
{
    public record UpdateWordBodyRequest(Guid Id, UpdateWordRequest Data);
}