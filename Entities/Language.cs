namespace VocabTrainer.Api.Entities
{
    public class Language
    {
        public string Id { get; set; } = default!;
        public string NameEn { get; set; } = default!;
        public string NameAr { get; set; } = default!;
        public string Locale { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}