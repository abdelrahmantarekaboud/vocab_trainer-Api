namespace VocabTrainer.Api.Abstractions.Storage
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(byte[] data, string path, string contentType, CancellationToken ct = default);
    }
}