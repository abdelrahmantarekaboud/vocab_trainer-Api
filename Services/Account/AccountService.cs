using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Mapping;
using VocabTrainer.Api.DataBase;
using Microsoft.EntityFrameworkCore;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Users;

namespace VocabTrainer.Api.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _db;

        public AccountService(ApplicationDbContext db) => _db = db;

        public async Task<Result<UserDto>> GetProfile(Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return Result<UserDto>.Failure(UserErrors.NotFound, "User not found");
            return Result<UserDto>.Success(user.ToDto());
        }

        public async Task<Result<UserDto>> UpdateSettings(Guid userId, UpdateSettingsRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return Result<UserDto>.Failure(UserErrors.NotFound, "User not found");

            if (req.UiLanguage != null) user.UiLanguage = req.UiLanguage;
            if (req.Theme != null) user.Theme = req.Theme;
            if (req.TtsSpeed != null) user.TtsSpeed = req.TtsSpeed.Value;
            if (req.TtsRepeatCount != null) user.TtsRepeatCount = req.TtsRepeatCount.Value;
            if (req.TtsVoiceId != null) user.TtsVoiceId = req.TtsVoiceId;
            if (req.CurrentLanguageId != null) user.CurrentLanguageId = req.CurrentLanguageId;

            await _db.SaveChangesAsync();
            return Result<UserDto>.Success(user.ToDto());
        }
    }
}