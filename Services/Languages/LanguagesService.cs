using Microsoft.EntityFrameworkCore;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Abstractions.Enums;
using VocabTrainer.Api.Contracts.Languages;
using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Mapping;

namespace VocabTrainer.Api.Services.Languages
{
    public class LanguagesService : ILanguagesService
    {
        private readonly ApplicationDbContext _db;

        public LanguagesService(ApplicationDbContext db) => _db = db;

        public async Task<Result<List<LanguageDto>>> GetAllowed(Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result<List<LanguageDto>>.Failure(UserErrors.NotFound, "User not found");

            var active = await _db.Languages.Where(l => l.IsActive).ToListAsync();

            var allowed = user.IsGuest
                ? active
                : active.Where(l => user.AllowedLanguages.Contains(l.Id)).ToList();

            return Result<List<LanguageDto>>.Success(allowed.Select(x => x.ToDto()).ToList());
        }

        public async Task<Result<List<LanguageDto>>> GetAllActive()
        {
            var active = await _db.Languages.Where(l => l.IsActive).ToListAsync();
            return Result<List<LanguageDto>>.Success(active.Select(x => x.ToDto()).ToList());
        }

        public async Task<Result<List<LanguageDto>>> RedeemLanguageCode(Guid userId, RedeemLanguageCodeRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Code))
                return Result<List<LanguageDto>>.Failure(CodeErrors.InvalidCode, "Code is required");

            var code = req.Code.Trim();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result<List<LanguageDto>>.Failure(UserErrors.NotFound, "User not found");

            if (user.IsGuest)
                return Result<List<LanguageDto>>.Failure(UserErrors.ForbiddenLanguage, "Guests cannot redeem language codes");

            var invite = await _db.InvitationCodes.FirstOrDefaultAsync(c =>
                c.Code == code &&
                c.IsActive &&
                c.Type == CodeType.AddLanguage
            );

            if (invite == null)
                return Result<List<LanguageDto>>.Failure(CodeErrors.InvalidCode, "Invalid or expired code");

            if (invite.ExpiresAt.HasValue && invite.ExpiresAt.Value <= DateTime.UtcNow)
                return Result<List<LanguageDto>>.Failure(CodeErrors.InvalidCode, "Code expired");

            bool unlimited = invite.MaxUses <= 0;

            if (!unlimited && invite.UsedCount >= invite.MaxUses)
                return Result<List<LanguageDto>>.Failure(CodeErrors.InvalidCode, "Code already used");

            if (user.Role != UserRole.User)
                return Result<List<LanguageDto>>.Failure(CodeErrors.InvalidCode, "Admins cannot redeem this code");


            if (invite.Languages == null || invite.Languages.Count == 0)
                return Result<List<LanguageDto>>.Failure(CodeErrors.InvalidCode, "Code has no languages");

            var langsToAdd = await _db.Languages
                .Where(l => l.IsActive && invite.Languages.Contains(l.Id))
                .Select(l => l.Id)
                .ToListAsync();

            if (langsToAdd.Count == 0)
                return Result<List<LanguageDto>>.Failure(LanguageErrors.NotFound, "No active languages in this code");

            foreach (var lid in langsToAdd)
            {
                if (!user.AllowedLanguages.Contains(lid))
                    user.AllowedLanguages.Add(lid);
            }

            invite.UsedCount++;

            if (!unlimited && invite.UsedCount >= invite.MaxUses)
            {
                invite.IsActive = false;
                _db.InvitationCodes.Remove(invite);
            }

            await _db.SaveChangesAsync();

            var allowed = await _db.Languages
                .Where(l => l.IsActive && user.AllowedLanguages.Contains(l.Id))
                .ToListAsync();

            return Result<List<LanguageDto>>.Success(allowed.Select(x => x.ToDto()).ToList());
        }


    }
}
