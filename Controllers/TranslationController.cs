using VocabTrainer.Api.Errors;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Services.Tts;
using VocabTrainer.Api.Contracts.Tts;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/translate")]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationProvider _translator;
        private readonly ITtsProvider _tts;

        public TranslationController(ITranslationProvider translator, ITtsProvider tts)
        {
            _translator = translator;
            _tts = tts;
        }

        [HttpPost]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Text))
                return BadRequest(Result.Failure(WordErrors.NotFound, "Text is required"));

            if (!_translator.IsSupported(req.From) || !_translator.IsSupported(req.To))
                return BadRequest(Result.Failure(LanguageErrors.NotFound, "Unsupported language"));

            try
            {
                var translated = await _translator.TranslateAsync(req.Text, req.From, req.To);
                return Ok(Result<string>.Success(translated));
            }
            catch (Exception ex)
            {
                // لو اللغة مش مدعومة فعلاً
                if (ex.Message.Contains("Unsupported language", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(Result.Failure(LanguageErrors.NotFound, ex.Message));

                // أي مشكلة من جوجل / الباكدج / ريت ليميت
                return BadRequest(Result.Failure(LanguageErrors.Disabled, ex.Message));
            }
        }
        [HttpPost("speak")]
        public async Task<IActionResult> Speak([FromBody] TtsRequest req, CancellationToken ct)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Text))
                return BadRequest(Result.Failure(WordErrors.NotFound, "Text is required"));

            if (string.IsNullOrWhiteSpace(req.LanguageCode))
                req = req with { LanguageCode = "en-US" };

            // safety clamps
            var repeat = Math.Clamp(req.Repeat, 1, 5);
            var rate = Math.Clamp(req.Rate, 0.5, 2.0);

            try
            {
                using var ms = new MemoryStream();

                for (int i = 0; i < repeat; i++)
                {
                    var wav = await _tts.SynthesizeAsync(
                        req.Text,
                        req.LanguageCode,
                        rate,
                        req.VoiceName,
                        ct);

                    await ms.WriteAsync(wav, 0, wav.Length, ct);
                }

                return File(ms.ToArray(), "audio/wav");
            }
            catch (Exception ex)
            {
                // ✅ لو السيرفر مفيهوش Voices خالص
                if (ex.Message.Contains("No voice installed", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("No voice installed on server", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(Result.Failure(
                        LanguageErrors.Disabled,
                        "TTS is not available on this server (no voices installed)."
                    ));
                }

                // ✅ لو لغة مش مدعومة عندك
                if (ex.Message.Contains("Unsupported language", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(Result.Failure(LanguageErrors.NotFound, "Unsupported language"));

                // أي خطأ تاني (بما فيه مشاكل TTS عامة)
                return BadRequest(Result.Failure(LanguageErrors.Disabled, ex.Message));
            }
        }



    }
}