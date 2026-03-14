namespace VocabTrainer.Api.Services.Tts
{
    public interface ITtsProvider
    {
        Task<byte[]> SynthesizeAsync(
            string text,
            string languageCode,
            double rate = 1.0,
            string? voiceName = null,
            CancellationToken ct = default);

        Task<List<(string Name, string Lang, string Gender)>> ListVoicesAsync(
            string? languageCode = null,
            CancellationToken ct = default);
    }
}
