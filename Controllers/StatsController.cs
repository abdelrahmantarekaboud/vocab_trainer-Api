using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Authentication;
using VocabTrainer.Api.Services.Stats;
using VocabTrainer.Api.Contracts.Stats;
using Microsoft.AspNetCore.Authorization;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/stats")]
    [Authorize]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _stats;
        private readonly ICurrentUserAccessor _current;

        public StatsController(IStatsService stats, ICurrentUserAccessor current)
        {
            _stats = stats;
            _current = current;
        }

        [HttpPost("summary")]
        public async Task<IActionResult> Summary([FromBody] StatsSummaryRequest req)
            => (await _stats.Summary(_current.UserId, req.LanguageId)).ToActionResult();
    }
}