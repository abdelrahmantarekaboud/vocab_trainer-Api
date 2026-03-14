using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VocabTrainer.Api.Entities;

public class UserLanguageStats
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string LanguageId { get; set; } = default!;

    public int TotalWords { get; set; }
    public int DueWords { get; set; }

    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public double Accuracy { get; set; }
    public int TranslatedWords { get; set; }
    public int UntranslatedWords { get; set; }

    public DateTime? LastQuizAtUtc { get; set; }
    public int? LastQuizTotalQuestions { get; set; }
    public int? LastQuizCorrectAnswers { get; set; }
    public double? LastQuizAccuracy { get; set; }

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}