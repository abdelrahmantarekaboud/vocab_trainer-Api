using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Admin;
using VocabTrainer.Api.Contracts.Users;

namespace VocabTrainer.Api.Services.Admin
{
    public interface IAdminUsersService
    {
        Task<Result<AdminUsersResponse>> ListUsers();
        Task<Result<AdminUserDto>> AdminUpdateUser(Guid userId, AdminUpdateUserRequest req);
        Task<Result> AdminResetPassword(Guid userId, string newPassword);
        Task<Result> Delete(Guid userId);
    }
}