/*
File Name: CurrentUserAccessor.cs
Author: Abdelrahman Tarek (ChatGPT helper)
Date of Creation: 06-12-2025
Version Information: 1.0.0
Dependencies: Microsoft.AspNetCore.Http, System.Security.Claims
Contributors: [Abdelrahman Tarek]
Last Modified Date: 06-12-2025
File Description:
Reads current authenticated user info from JWT claims.
Supports "sub" claim + NameIdentifier + custom keys.
*/

using System.Security.Claims;
using VocabTrainer.Api.Abstractions;

namespace VocabTrainer.Api.Authentication
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserAccessor(IHttpContextAccessor http)
        {
            _http = http;
        }

        public bool IsAuthenticated
            => _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public Guid UserId
        {
            get
            {
                var user = _http.HttpContext?.User;

                if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                    throw new UnauthorizedAccessException("User not authenticated");

                // ✅ Support multiple claim keys
                var idStr =
                    user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    user.FindFirstValue("sub") ??          // <-- ده اللي عندك في التوكن
                    user.FindFirstValue("userId") ??
                    user.FindFirstValue("id");

                if (string.IsNullOrWhiteSpace(idStr))
                    throw new UnauthorizedAccessException("UserId claim is missing");

                if (!Guid.TryParse(idStr, out var id))
                    throw new UnauthorizedAccessException("UserId claim is invalid");

                return id;
            }
        }

        public string Role
            => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
               ?? _http.HttpContext?.User?.FindFirstValue("role")
               ?? "User";

        public string? Email
            => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
               ?? _http.HttpContext?.User?.FindFirstValue("email");
    }
}