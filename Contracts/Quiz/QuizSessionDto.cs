using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Contracts.Quiz
{
    public record QuizSessionDto(Guid Id, string LanguageId, QuizMode Mode, string? TopicFilter, DateTime StartedAt);
}