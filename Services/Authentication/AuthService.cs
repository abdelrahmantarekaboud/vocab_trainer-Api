using VocabTrainer.Api.Errors;
using VocabTrainer.Api.Helpers;
using VocabTrainer.Api.Mapping;
using VocabTrainer.Api.Entities;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Auth;
using VocabTrainer.Api.Abstractions.Enums;

namespace VocabTrainer.Api.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtProvider _jwt;

        public AuthService(ApplicationDbContext db, IJwtProvider jwt)
        {
            _db = db; _jwt = jwt;
        }

        public async Task<Result<AuthResponse>> Login(LoginRequest req)
        {
            var email = req.Email.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null || !PasswordHasherHelper.Verify(req.Password, user.PasswordHash))
                return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials, "Invalid email or password");

            var access = _jwt.GenerateAccessToken(user);
            var refresh = _jwt.GenerateRefreshToken(user);
            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            return Result<AuthResponse>.Success(new AuthResponse(access, refresh.Token, user.ToDto()));
        }

        public async Task<Result<AuthResponse>> Guest()
        {
            var activeLangs = await _db.Languages.Where(l => l.IsActive).Select(l => l.Id).ToListAsync();
            if (activeLangs.Count == 0) activeLangs.Add("en");

            var guest = new User
            {
                Name = "Guest",
                Email = $"guest-{Guid.NewGuid():N}@guest.local",
                PasswordHash = PasswordHasherHelper.Hash(Guid.NewGuid().ToString()),
                IsGuest = true,
                Role = UserRole.User,
                AllowedLanguages = activeLangs,
                CurrentLanguageId = activeLangs.First()
            };

            _db.Users.Add(guest);

            var refresh = _jwt.GenerateRefreshToken(guest);
            _db.RefreshTokens.Add(refresh);

            await _db.SaveChangesAsync();

            var access = _jwt.GenerateAccessToken(guest);
            return Result<AuthResponse>.Success(new AuthResponse(access, refresh.Token, guest.ToDto()));
        }

        public async Task<Result<AuthResponse>> RegisterWithCode(RegisterWithCodeRequest req)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            var code = await _db.InvitationCodes
                .FromSqlRaw("SELECT * FROM InvitationCodes WITH (UPDLOCK, ROWLOCK) WHERE Code = {0}", req.Code)
                .FirstOrDefaultAsync();

            if (code == null || !code.IsActive)
                return Result<AuthResponse>.Failure(CodeErrors.InvalidCode, "Invalid code");

            if (code.Type != CodeType.InviteAccount)
                return Result<AuthResponse>.Failure(CodeErrors.NotInvite, "Not an invite code");

            if (DateTimeHelper.IsExpired(code.ExpiresAt))
                return Result<AuthResponse>.Failure(CodeErrors.ExpiredCode, "Code expired");

            if (code.UsedCount >= code.MaxUses)
                return Result<AuthResponse>.Failure(CodeErrors.UsedCode, "Code already used");

            var email = req.Email.Trim().ToLower();
            if (await _db.Users.AnyAsync(x => x.Email == email))
                return Result<AuthResponse>.Failure(UserErrors.EmailExists, "Email already exists");

            var user = new User
            {
                Name = req.Name.Trim(),
                Email = email,
                PasswordHash = PasswordHasherHelper.Hash(req.Password),
                Role = code.TargetRole,
                AllowedLanguages = code.Languages.ToList(),
                CurrentLanguageId = code.Languages.First()
            };

            _db.Users.Add(user);

            // consume
            code.UsedCount = 1;
            code.IsActive = false;

            _db.InvitationRedemptions.Add(new InvitationRedemption
            {
                InvitationCodeId = code.Id,
                UserId = user.Id,
                RedeemerEmail = user.Email
            });

            var refresh = _jwt.GenerateRefreshToken(user);
            _db.RefreshTokens.Add(refresh);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var access = _jwt.GenerateAccessToken(user);
            return Result<AuthResponse>.Success(new AuthResponse(access, refresh.Token, user.ToDto()));
        }

        public async Task<Result<AuthResponse>> Refresh(RefreshRequest req)
        {
            var rt = await _db.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == req.RefreshToken);

            if (rt == null || rt.IsRevoked || rt.ExpiresAt <= DateTime.UtcNow)
                return Result<AuthResponse>.Failure("refresh.invalid", "Invalid refresh token");

            var user = await _db.Users.FirstAsync(x => x.Id == rt.UserId);
            rt.IsRevoked = true;

            var newRt = _jwt.GenerateRefreshToken(user);
            _db.RefreshTokens.Add(newRt);

            await _db.SaveChangesAsync();

            var access = _jwt.GenerateAccessToken(user);
            return Result<AuthResponse>.Success(new AuthResponse(access, newRt.Token, user.ToDto()));
        }
    }
}