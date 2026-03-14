namespace VocabTrainer.Api.Contracts.Tts
{
    public record TtsRequest(
            string Text,
            string LanguageCode,
            double Rate = 1.0,
            int Repeat = 1,
            string? VoiceName = null
        );
}