using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Words;

namespace VocabTrainer.Api.Services.Words
{
    public interface IWordsService
    {
        Task<Result<WordDto>> Create(Guid userId, CreateWordRequest req);

        Task<Result<List<WordDto>>> List(Guid userId, WordsQuery q);

        Task<Result<WordDto>> Update(Guid userId, Guid wordId, UpdateWordRequest req);

        Task<Result> Delete(Guid userId, Guid wordId);

        Task<Result<WordDto>> SaveRecording(Guid userId, SaveRecordingRequest req);

        Task<Result<(byte[] bytes, string contentType)>> GetRecording(Guid userId, Guid wordId);

        Task<Result<int>> DeleteAllForUser(Guid userId);
        Task<Result<WordDto>> DeleteRecording(Guid userId, Guid wordId);


    }
}