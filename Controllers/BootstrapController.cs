using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Mapping;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Users;
using Microsoft.AspNetCore.Authorization;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/bootstrap")]
    [Authorize]
    public class BootstrapController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserAccessor _current;

        public BootstrapController(ApplicationDbContext db, ICurrentUserAccessor current)
        {
            _db = db;
            _current = current;
        }

        public record BootstrapRequest();

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var userId = _current.UserId;

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return Unauthorized();

            var active = await _db.Languages.Where(l => l.IsActive).ToListAsync();
            var allowed = user.IsGuest
                ? active
                : active.Where(l => user.AllowedLanguages.Contains(l.Id)).ToList();

            if (allowed.Count > 0 && !user.AllowedLanguages.Contains(user.CurrentLanguageId))
            {
                user.CurrentLanguageId = allowed[0].Id;
                await _db.SaveChangesAsync();
            }

            var res = new BootstrapResponse(user.ToDto(), allowed.Select(x => x.ToDto()).ToList());
            return Ok(res);
        }
    }
}