using System.Speech.Synthesis;

namespace VocabTrainer.Api.Services.Tts
{
    public class WindowsTtsProvider : ITtsProvider
    {
        private static readonly HashSet<string> SupportedLangs =
            new(StringComparer.OrdinalIgnoreCase) { "en", "de", "fr", "es", "it", "ar" };

        public Task<byte[]> SynthesizeAsync(
            string text,
            string languageCode,
            double rate = 1.0,
            string? voiceName = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new Exception("Text is required");

            languageCode = string.IsNullOrWhiteSpace(languageCode) ? "en-US" : languageCode.Trim();
            var shortLang = languageCode.Split('-')[0].ToLower();

            if (!SupportedLangs.Contains(shortLang))
                throw new Exception("Unsupported language");

            using var synth = new SpeechSynthesizer();

            var installed = synth.GetInstalledVoices()
                .Select(v => v.VoiceInfo)
                .ToList();

            if (installed.Count == 0)
                throw new Exception("No voice installed on server.");

            // 1) لو voiceName معين و موجود
            if (!string.IsNullOrWhiteSpace(voiceName))
            {
                var named = installed.FirstOrDefault(v =>
                    v.Name.Equals(voiceName, StringComparison.OrdinalIgnoreCase));

                if (named != null)
                    synth.SelectVoice(named.Name);
            }

            // 2) لو لسه مفيش صوت متحدد → اختار صوت بنفس اللغة
            if (synth.Voice == null || string.IsNullOrEmpty(synth.Voice.Name))
            {
                var match = installed.FirstOrDefault(v =>
                    v.Culture.Name.StartsWith(shortLang, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                    synth.SelectVoice(match.Name);
                else
                    // 3) لو مفيش صوت للغة → اختار أول صوت موجود (fallback)
                    synth.SelectVoice(installed[0].Name);
            }

            // ✅ Speed mapping
            var mappedRate = (int)Math.Round((rate - 1.0) * 10);
            synth.Rate = Math.Clamp(mappedRate, -10, 10);

            using var ms = new MemoryStream();
            synth.SetOutputToWaveStream(ms);
            synth.Speak(text);

            return Task.FromResult(ms.ToArray()); // WAV
        }

        public Task<List<(string Name, string Lang, string Gender)>> ListVoicesAsync(
            string? languageCode = null,
            CancellationToken ct = default)
        {
            using var synth = new SpeechSynthesizer();

            var voices = synth.GetInstalledVoices()
                .Select(v => (
                    Name: v.VoiceInfo.Name,
                    Lang: v.VoiceInfo.Culture.Name,
                    Gender: v.VoiceInfo.Gender.ToString()
                ))
                .ToList();

            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                var shortLang = languageCode.Split('-')[0];
                voices = voices
                    .Where(v => v.Lang.StartsWith(shortLang, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Task.FromResult(
                voices.Select(v => (v.Name, v.Lang, v.Gender)).ToList()
            );
        }
    }
}
