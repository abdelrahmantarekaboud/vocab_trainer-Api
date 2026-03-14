using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Contracts.Quiz
{
    public record CreateQuizSessionRequest(
         string LanguageId,
         QuizMode Mode,
         string? TopicFilter = null
     );
}