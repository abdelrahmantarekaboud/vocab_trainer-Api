using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using Microsoft.AspNetCore.Authorization;
using VocabTrainer.Api.Contracts.Languages;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/languages")]
    [Authorize]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguagesService _langs;
        private readonly ICurrentUserAccessor _current;

        public LanguagesController(ILanguagesService langs, ICurrentUserAccessor current)
        {
            _langs = langs;
            _current = current;
        }

        [HttpPost("allowed")]
        public async Task<IActionResult> Allowed([FromBody] AllowedLanguagesRequest _)
            => (await _langs.GetAllowed(_current.UserId)).ToActionResult();

        [HttpPost("redeem")]
        public async Task<IActionResult> Redeem([FromBody] RedeemLanguageCodeRequest req)
           => (await _langs.RedeemLanguageCode(_current.UserId, req)).ToActionResult();

    }
}
