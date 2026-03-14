using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Entities
{
    public class QuizSession
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string LanguageId { get; set; } = default!;

        public QuizMode Mode { get; set; }

        public string? TopicFilter { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        public int TotalItems { get; set; }

        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int Score { get; set; }
    }
}
