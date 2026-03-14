namespace VocabTrainer.Api.Abstractions
{
    public class Result
    {
        public bool Succeeded { get; init; }
        public Error? Error { get; init; }

        public static Result Success() => new() { Succeeded = true };

        public static Result Failure(string code, string message)
            => new() { Succeeded = false, Error = new Error(code, message) };
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }

        public static Result<T> Success(T value)
            => new() { Succeeded = true, Value = value };

        public new static Result<T> Failure(string code, string message)
            => new() { Succeeded = false, Error = new Error(code, message) };
    }
}