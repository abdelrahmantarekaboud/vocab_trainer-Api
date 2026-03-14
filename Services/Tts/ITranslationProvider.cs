namespace VocabTrainer.Api.Services.Tts
{
    public interface ITranslationProvider
    {
        Task<string> TranslateAsync(string text, string from, string to, CancellationToken ct = default);

        bool IsSupported(string langCode);

    }
}