using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Authentication;
using VocabTrainer.Api.Contracts.Words;
using VocabTrainer.Api.Services.Words;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/words")]
    [Authorize]
    public class WordsController : ControllerBase
    {
        private readonly IWordsService _words;
        private readonly ICurrentUserAccessor _current;

        public WordsController(IWordsService words, ICurrentUserAccessor current)
        {
            _words = words;
            _current = current;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateWordRequest req)
            => (await _words.Create(_current.UserId, req)).ToActionResult();

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] WordsQuery q)
            => (await _words.List(_current.UserId, q)).ToActionResult();


        [HttpPatch("update")]
        public async Task<IActionResult> Update([FromBody] UpdateWordBodyRequest req)
            => (await _words.Update(_current.UserId, req.Id, req.Data)).ToActionResult();

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteWordRequest req)
            => (await _words.Delete(_current.UserId, req.WordId)).ToActionResult();

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll()
            => (await _words.DeleteAllForUser(_current.UserId)).ToActionResult();

        [HttpPost("recording/save")]
        public async Task<IActionResult> SaveRecording([FromBody] SaveRecordingRequest req)
            => (await _words.SaveRecording(_current.UserId, req)).ToActionResult();

        [HttpPost("recording/get")]
        public async Task<IActionResult> GetRecording([FromBody] GetRecordingRequest req)
        {
            var res = await _words.GetRecording(_current.UserId, req.WordId);
            if (!res.Succeeded) return res.ToActionResult();

            var (bytes, contentType) = res.Value;
            return File(bytes, contentType);
        }
        [HttpDelete("recording/delete/{wordId:guid}")]
        public async Task<IActionResult> DeleteRecording([FromRoute] Guid wordId)
        {
            var userId = _current.UserId;
            var res = await _words.DeleteRecording(userId, wordId);
            return res.ToActionResult();
        }



    }
}
