namespace VocabTrainer.Api.Contracts.Common
{
    public record ApiResponse<T>(bool Succeeded, T? Data, object? Error)
    {
        public static ApiResponse<T> Ok(T data) => new(true, data, null);
        public static ApiResponse<T> Fail(object error) => new(false, default, error);
    }
}