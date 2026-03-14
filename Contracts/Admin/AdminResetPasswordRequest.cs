namespace VocabTrainer.Api.Contracts.Admin;

public record AdminResetPasswordRequest(Guid Id, string NewPassword);
