namespace VocabTrainer.Api.Contracts.Admin;

public record AddLanguageByNameRequest(
      string NameEn,
      bool IsActive = true
  );
