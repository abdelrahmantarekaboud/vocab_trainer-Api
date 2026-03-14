using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Admin;
using VocabTrainer.Api.Contracts.Languages;

namespace VocabTrainer.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/languages")]
    [Authorize(Roles = "Admin")]
    public class AdminLanguagesController : ControllerBase
    {
        private readonly IAdminLanguagesService _langs;

        public AdminLanguagesController(IAdminLanguagesService langs)
            => _langs = langs;

        // -------------------- List All Languages --------------------

        [HttpPost("list")]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromBody] AdminListLanguagesRequest _)
            => (await _langs.ListAll()).ToActionResult();

        // -------------------- Toggle Language Active/Disabled --------------------
        [HttpPatch("toggle")]
        public async Task<IActionResult> Toggle([FromBody] ToggleLanguageRequest req)
            => (await _langs.Toggle(req.Id, req.Active)).ToActionResult();

        // -------------------- Add Language (Admin Only) --------------------
        [HttpPost("add-language")]
        public async Task<IActionResult> AddLanguage([FromBody] AddLanguageByNameRequest request)
            => (await _langs.AddLanguageByName(request)).ToActionResult();

        // -------------------- delete Language (Admin Only) --------------------

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteLanguageRequest req)
    => (await _langs.Delete(req.Id)).ToActionResult();

    }
}
