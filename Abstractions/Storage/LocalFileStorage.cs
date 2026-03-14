using VocabTrainer.Api.Abstractions.Storage;

namespace VocabTrainer.Api.Storage
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileStorage(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveAsync(byte[] data, string path, string contentType, CancellationToken ct = default)
        {
            var root = Path.Combine(_env.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(root);

            var fullPath = Path.Combine(root, path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            await File.WriteAllBytesAsync(fullPath, data, ct);

            // URL relative
            return "/" + path.Replace("\\", "/");
        }
    }
}