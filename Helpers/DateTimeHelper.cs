namespace VocabTrainer.Api.Helpers
{
    public static class DateTimeHelper
    {
        public static bool IsExpired(DateTime? dt) =>
            dt != null && dt <= DateTime.UtcNow;
    }
}