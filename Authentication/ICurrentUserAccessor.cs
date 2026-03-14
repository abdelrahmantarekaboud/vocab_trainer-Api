/*
File Name: ICurrentUserAccessor.cs
Author: Abdelrahman Tarek (ChatGPT helper)
Date of Creation: 06-12-2025
Version Information: 1.0.0
Dependencies: Microsoft.AspNetCore.Http, System
Contributors: [Abdelrahman Tarek]
Last Modified Date: 06-12-2025
File Description:
Abstraction for accessing current authenticated user info (Id, Role, Email) from HttpContext claims.
*/

namespace VocabTrainer.Api.Abstractions
{
    public interface ICurrentUserAccessor
    {
        Guid UserId { get; }     // Guid بتاع اليوزر الحالي
        string Role { get; }     // Role (Admin/User/Guest ...)
        string? Email { get; }   // Email (اختياري)
        bool IsAuthenticated { get; } // هل المستخدم عامل Login ولا لأ
    }
}