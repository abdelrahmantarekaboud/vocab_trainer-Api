using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Helpers;
using VocabTrainer.Api.Mapping;
using VocabTrainer.Api.DataBase;
using VocabTrainer.Api.Entities;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Admin;
using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Services.Admin
{
    public class AdminCodesService : IAdminCodesService
    {
        private readonly ApplicationDbContext _db;

        public AdminCodesService(ApplicationDbContext db) => _db = db;

        public async Task<Result<List<CodeDto>>> GenerateInviteCodes(Guid adminId, GenerateInviteCodesRequest req)
        {
            if (req == null)
                return Result<List<CodeDto>>.Failure(WordErrors.NotFound, "Request is required");

            var langs = req.Languages?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLower())
                .Distinct()
                .ToList() ?? new List<string>();

            if (langs.Count == 0)
                return Result<List<CodeDto>>.Failure(LanguageErrors.Disabled, "No languages selected");

            var active = await _db.Languages
                .Where(l => l.IsActive)
                .Select(l => l.Id.Trim().ToLower())
                .ToHashSetAsync();

            if (langs.Any(x => !active.Contains(x)))
                return Result<List<CodeDto>>.Failure(LanguageErrors.Disabled, "Some languages disabled");

            var count = Math.Clamp(req.Count, 1, 100);

            var list = new List<InvitationCode>();
            var usedCodes = new HashSet<string>();

            for (int i = 0; i < count; i++)
            {
                string code;
                do
                {
                    code = CodeGenerator.Generate("VT");
                }
                while (!usedCodes.Add(code));

                list.Add(new InvitationCode
                {
                    Code = code,
                    Type = CodeType.InviteAccount,
                    TargetRole = req.TargetRole,
                    Languages = langs,
                    MaxUses = 1,
                    UsedCount = 0,
                    IsActive = true,
                    ExpiresAt = req.ExpiresAt,
                    CreatedByAdminId = adminId
                });
            }

            _db.InvitationCodes.AddRange(list);
            await _db.SaveChangesAsync();

            return Result<List<CodeDto>>.Success(list.Select(x => x.ToDto()).ToList());
        }



        public async Task<Result<List<CodeDto>>> GenerateLanguageCodes(Guid adminId, GenerateLanguageCodesRequest req)
        {
            var active = await _db.Languages.Where(l => l.IsActive).Select(l => l.Id).ToHashSetAsync();
            if (req.Languages.Count == 0 || req.Languages.Any(x => !active.Contains(x)))
                return Result<List<CodeDto>>.Failure(LanguageErrors.Disabled, "Some languages disabled");

            var list = new List<InvitationCode>();
            for (int i = 0; i < req.Count; i++)
            {
                list.Add(new InvitationCode
                {
                    Code = CodeGenerator.Generate("LANG"),
                    Type = CodeType.AddLanguage,
                    Languages = req.Languages.Select(x => x.Trim().ToLowerInvariant()).ToList(),
                    MaxUses = 1,
                    UsedCount = 0,
                    IsActive = true,
                    ExpiresAt = req.ExpiresAt,
                    CreatedByAdminId = adminId,
                    TargetUserId = null,
                    TargetRole = UserRole.User
                });
            }

            _db.InvitationCodes.AddRange(list);
            await _db.SaveChangesAsync();

            return Result<List<CodeDto>>.Success(list.Select(x => x.ToDto()).ToList());
        }

        public async Task<Result<AdminCodesResponse>> ListCodesGrouped()
        {
            var list = await _db.InvitationCodes
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var groups = list
                .GroupBy(c => c.Type)
                .Select(g => new CodesGroupDto(
                    Type: g.Key.ToString(),
                    Codes: g.Select(x => x.ToDto()).ToList()
                ))
                .ToList();

            return Result<AdminCodesResponse>.Success(new AdminCodesResponse(groups));
        }

        public async Task<Result> Disable(Guid id)
        {
            var c = await _db.InvitationCodes.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return Result.Failure(CodeErrors.InvalidCode, "Code not found");

            c.IsActive = false;
            await _db.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result> Delete(Guid id)
        {
            var c = await _db.InvitationCodes.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null)
                return Result.Failure(CodeErrors.InvalidCode, "Code not found");

            _db.InvitationCodes.Remove(c);
            await _db.SaveChangesAsync();

            return Result.Success();
        }


    }
}