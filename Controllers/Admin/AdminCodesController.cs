using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Authentication;
using VocabTrainer.Api.Services.Admin;
using VocabTrainer.Api.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;

namespace VocabTrainer.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/codes")]
    [Authorize(Roles = "Admin")]
    public class AdminCodesController : ControllerBase
    {
        private readonly IAdminCodesService _codes;
        private readonly ICurrentUserAccessor _current;

        public AdminCodesController(IAdminCodesService codes, ICurrentUserAccessor current)
        {
            _codes = codes; _current = current;
        }

        [HttpPost("invite")]
        public async Task<IActionResult> Invite([FromBody] GenerateInviteCodesRequest req)
            => (await _codes.GenerateInviteCodes(_current.UserId, req)).ToActionResult();

        [HttpPost("languages")]
        public async Task<IActionResult> Languages([FromBody] GenerateLanguageCodesRequest req)
            => (await _codes.GenerateLanguageCodes(_current.UserId, req)).ToActionResult();

        [HttpGet("list-grouped")]
        public async Task<IActionResult> ListGrouped()
            => (await _codes.ListCodesGrouped()).ToActionResult();

        public record DisableCodeRequest(Guid Id);

        [HttpPatch("disable")]
        public async Task<IActionResult> Disable([FromBody] DisableCodeRequest req)
            => (await _codes.Disable(req.Id)).ToActionResult();

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DisableCodeRequest req)
    => (await _codes.Delete(req.Id)).ToActionResult();


    }
}