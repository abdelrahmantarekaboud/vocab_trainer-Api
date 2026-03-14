namespace VocabTrainer.Api.Contracts.Tts
{
    public record TtsVoiceDto(string Name, string LanguageCode, string Gender);

    public record TtsVoicesResponse(List<TtsVoiceDto> Voices);
}
