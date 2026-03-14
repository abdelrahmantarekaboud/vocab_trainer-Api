using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Authentication;
using VocabTrainer.Api.Contracts.Users;
using VocabTrainer.Api.Services.Account;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _acc;
        private readonly ICurrentUserAccessor _current;

        public AccountController(IAccountService acc, ICurrentUserAccessor current)
        {
            _acc = acc; _current = current;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
            => (await _acc.GetProfile(_current.UserId)).ToActionResult();

        [HttpPatch("settings")]
        public async Task<IActionResult> Settings([FromBody] UpdateSettingsRequest req)
            => (await _acc.UpdateSettings(_current.UserId, req)).ToActionResult();
    }
}
