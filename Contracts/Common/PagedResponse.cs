namespace VocabTrainer.Api.Contracts.Common
{
    public record PagedResponse<T>(
        List<T> Items,
        int PageNumber,
        int PageSize,
        int TotalCount
    );
}