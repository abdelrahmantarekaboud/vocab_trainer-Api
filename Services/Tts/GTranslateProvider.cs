using GTranslate.Translators;
using Microsoft.Extensions.Logging;

namespace VocabTrainer.Api.Services.Tts
{
    public class GTranslateProvider : ITranslationProvider
    {
        private readonly ILogger<GTranslateProvider> _logger;
        private readonly GoogleTranslator _translator;

        public GTranslateProvider(ILogger<GTranslateProvider> logger)
        {
            _logger = logger;
            _translator = new GoogleTranslator();
        }

        public bool IsSupported(string langCode)
        {
            return !string.IsNullOrWhiteSpace(langCode);
        }


        public async Task<string> TranslateAsync(string text, string from, string to, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new Exception("Text is required");

            from = from.Trim().ToLower();
            to = to.Trim().ToLower();

            if (from == to) return text;

            try
            {
                var result = await _translator.TranslateAsync(text, to, from);
                var translated = result.Translation;

                if (string.IsNullOrWhiteSpace(translated))
                    throw new Exception("No translation returned");

                return translated;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("GTranslate failed: {Msg}", ex.Message);
                throw new Exception("Free Google translate is temporarily unavailable.");
            }
        }

    }
}