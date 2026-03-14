namespace VocabTrainer.Api.Settings
{
    public class ElevenLabsSettings
    {
        public const string SectionName = "ElevenLabs";
        public string ApiKey { get; set; } = default!;
        public string DefaultVoiceId { get; set; } = default!;
    }
}