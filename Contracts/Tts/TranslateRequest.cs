namespace VocabTrainer.Api.Contracts.Tts
{
    public record TranslateRequest(string Text, string From, string To);
}