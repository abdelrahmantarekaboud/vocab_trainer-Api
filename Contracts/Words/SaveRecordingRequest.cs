namespace VocabTrainer.Api.Contracts.Words
{
    public record SaveRecordingRequest(
        Guid WordId,
        string Base64Audio,
        string? FileExt
    );
}