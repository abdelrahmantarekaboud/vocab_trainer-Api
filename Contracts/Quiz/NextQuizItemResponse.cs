using VocabTrainer.Api.Contracts.Words;

namespace VocabTrainer.Api.Contracts.Quiz
{
    public record NextQuizItemResponse(
           WordDto? Item,
           List<string> Choices,
           string? CorrectAnswer,
           int Attempt,
           int Total,
           int Answered,
           int CorrectCount,
           int WrongCount,
           int Score,
           bool IsFinished
       );
}