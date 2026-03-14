using VocabTrainer.Api.Entities;

namespace VocabTrainer.Api.Helpers
{
    public static class SpacedRepetitionHelper
    {
        public static void ApplyAnswer(Word w, bool correct)
        {
            w.ReviewCount++;
            w.LastReviewedAt = DateTime.UtcNow;

            if (correct)
            {
                w.CorrectCount++;
                w.EaseFactor = Math.Min(2.5, w.EaseFactor + 0.1);
                w.IntervalDays = Math.Max(1, (int)Math.Round(w.IntervalDays * w.EaseFactor));
            }
            else
            {
                w.WrongCount++;
                w.EaseFactor = Math.Max(1.3, w.EaseFactor - 0.2);
                w.IntervalDays = 1;
            }

            w.NextReviewAt = DateTime.UtcNow.AddDays(w.IntervalDays);
            w.UpdatedAt = DateTime.UtcNow;
        }
    }
}