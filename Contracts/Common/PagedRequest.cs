namespace VocabTrainer.Api.Contracts.Common
{
    public record PagedRequest(int PageNumber = 1, int PageSize = 20);
}