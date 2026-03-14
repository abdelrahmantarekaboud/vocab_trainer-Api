using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Admin;

namespace VocabTrainer.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUsersService _users;

        public AdminUsersController(IAdminUsersService users)
            => _users = users;

        // -------------------- List Users --------------------
        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] AdminListUsersRequest _)
            => (await _users.ListUsers()).ToActionResult();

        // -------------------- Update User (AdminUpdate) --------------------
        [HttpPatch("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserBodyRequest req)
            => (await _users.AdminUpdateUser(req.Id, req.Data)).ToActionResult();

        // -------------------- Reset Password --------------------
        public record ResetPasswordRequest(Guid Id, string NewPassword);

        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
            => (await _users.AdminResetPassword(req.Id, req.NewPassword)).ToActionResult();

        // -------------------- Delete User --------------------
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteUserRequest req)
            => (await _users.Delete(req.Id)).ToActionResult();
    }
}
