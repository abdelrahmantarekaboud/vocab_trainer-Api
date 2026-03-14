namespace VocabTrainer.Api.Entities
{
    public class Word
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string LanguageId { get; set; } = default!;

        public string Text { get; set; } = default!;
        public string? Translation { get; set; }
        public string? Example { get; set; }
        public string? Topic { get; set; }
        public string? RecordingUrl { get; set; }

        public int ReviewCount { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public DateTime? LastReviewedAt { get; set; }

        public double EaseFactor { get; set; } = 2.0;
        public int IntervalDays { get; set; } = 1;
        public DateTime? NextReviewAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}