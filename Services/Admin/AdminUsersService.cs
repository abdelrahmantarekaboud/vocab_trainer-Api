using Microsoft.EntityFrameworkCore;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Abstractions.Enums;
using VocabTrainer.Api.Contracts.Admin;
using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Mapping;

namespace VocabTrainer.Api.Services.Admin
{
    public class AdminUsersService : IAdminUsersService
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersService(ApplicationDbContext db) => _db = db;

        public async Task<Result<AdminUsersResponse>> ListUsers()
        {
            var users = await _db.Users
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var total = users.Count;
            var totalAdmins = users.Count(u => u.Role == UserRole.Admin);
            var totalNormalUsers = users.Count(u => u.Role == UserRole.User);
            var totalGuests = users.Count(u => u.IsGuest);

            var response = new AdminUsersResponse(
                Users: users.Select(x => x.ToAdminDto()).ToList(),
                TotalUsers: total,
                TotalAdmins: totalAdmins,
                TotalNormalUsers: totalNormalUsers,
                TotalGuests: totalGuests
            );

            return Result<AdminUsersResponse>.Success(response);
        }

        public async Task<Result<AdminUserDto>> AdminUpdateUser(Guid userId, AdminUpdateUserRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result<AdminUserDto>.Failure(UserErrors.NotFound, "User not found");

            if (req.Name != null)
                user.Name = req.Name.Trim();

            if (req.Email != null)
            {
                var email = req.Email.Trim().ToLowerInvariant();

                var exists = await _db.Users.AnyAsync(u => u.Email == email && u.Id != userId);
                if (exists)
                    return Result<AdminUserDto>.Failure(UserErrors.DuplicateEmail, "Email already in use");

                user.Email = email;
                user.EmailConfirmed = false; // منطقي لما الإيميل يتغير
            }

            if (req.Role != null)
                user.Role = Enum.Parse<UserRole>(req.Role, true);

            if (req.IsGuest.HasValue)
                user.IsGuest = req.IsGuest.Value;

            if (req.EmailConfirmed.HasValue)
                user.EmailConfirmed = req.EmailConfirmed.Value;

            if (req.AllowedLanguages != null)
                user.AllowedLanguages = req.AllowedLanguages;

            if (req.CurrentLanguageId != null)
                user.CurrentLanguageId = req.CurrentLanguageId.Trim().ToLowerInvariant();

            if (req.UiLanguage != null)
                user.UiLanguage = req.UiLanguage.Trim().ToLowerInvariant();

            if (req.Theme != null)
                user.Theme = req.Theme.Trim().ToLowerInvariant();

            if (req.TtsSpeed.HasValue)
                user.TtsSpeed = Math.Clamp(req.TtsSpeed.Value, 0.5, 2.0);

            if (req.TtsRepeatCount.HasValue)
                user.TtsRepeatCount = Math.Clamp(req.TtsRepeatCount.Value, 1, 5);

            if (req.TtsVoiceId != null)
                user.TtsVoiceId = string.IsNullOrWhiteSpace(req.TtsVoiceId) ? null : req.TtsVoiceId.Trim();

            if (!string.IsNullOrWhiteSpace(req.NewPassword))
            {
                var newPassword = req.NewPassword.Trim();

                if (newPassword.Length < 8)
                    return Result<AdminUserDto>.Failure(UserErrors.InvalidPassword, "Password too short");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }


            await _db.SaveChangesAsync();
            return Result<AdminUserDto>.Success(user.ToAdminDto());
        }

        public async Task<Result> AdminResetPassword(Guid userId, string newPassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result.Failure(UserErrors.NotFound, "User not found");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Trim().Length < 8)
                return Result.Failure(UserErrors.InvalidPassword, "Password too short");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword.Trim());
            await _db.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> Delete(Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return Result.Failure(UserErrors.NotFound, "User not found");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return Result.Success();
        }
    }
}
