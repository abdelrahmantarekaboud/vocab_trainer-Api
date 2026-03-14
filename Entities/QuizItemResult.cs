namespace VocabTrainer.Api.Entities
{
    public class QuizItemResult
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public Guid WordId { get; set; }

        public Guid UserId { get; set; }

        public string LanguageId { get; set; } = default!;

        public bool IsCorrect { get; set; }
        public int ResponseTimeMs { get; set; }

        public int Attempt { get; set; } = 1;
        public string? SelectedAnswer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
