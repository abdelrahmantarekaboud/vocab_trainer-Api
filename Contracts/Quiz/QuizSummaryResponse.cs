using VocabTrainer.Api.Contracts.Words;

namespace VocabTrainer.Api.Contracts.Quiz;

public record QuizSummaryResponse(
    int Total,
    int CorrectCount,
    int WrongCount,
    int Score,
    List<WordDto> WrongItems
);