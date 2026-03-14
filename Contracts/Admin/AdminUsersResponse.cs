using VocabTrainer.Api.Contracts.Users;

namespace VocabTrainer.Api.Contracts.Admin
{
    public record AdminUsersResponse(
        List<AdminUserDto> Users,
        int TotalUsers,
        int TotalAdmins,
        int TotalNormalUsers,
        int TotalGuests
    );
}