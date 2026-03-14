using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Auth;
using VocabTrainer.Api.Services.Authentication;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthenticationController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
            => (await _auth.Login(req)).ToActionResult();

        [HttpPost("register-with-code")]
        public async Task<IActionResult> Register([FromBody] RegisterWithCodeRequest req)
            => (await _auth.RegisterWithCode(req)).ToActionResult();

        [HttpPost("guest")]
        public async Task<IActionResult> Guest()
            => (await _auth.Guest()).ToActionResult();

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequest req)
            => (await _auth.Refresh(req)).ToActionResult();
    }
}