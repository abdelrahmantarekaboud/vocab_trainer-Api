using System.ComponentModel.DataAnnotations;

namespace VocabTrainer.Api.Authentication
{
    public class JwtOptions
    {
        public const string SectionName = "JwtSettings";

        [Required] public string Issuer { get; set; } = default!;
        [Required] public string Audience { get; set; } = default!;
        [Required] public string Key { get; set; } = default!;
        public int ExpiryMinutes { get; set; } = 60;
        public int RefreshExpiryDays { get; set; } = 30;
    }
}