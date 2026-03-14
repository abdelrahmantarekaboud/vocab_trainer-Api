namespace VocabTrainer.Api.Contracts.Admin;

public record CodesGroupDto(string Type, List<CodeDto> Codes);

public record AdminCodesResponse(List<CodesGroupDto> Groups);
