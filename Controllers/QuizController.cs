using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Contracts.Quiz;
using VocabTrainer.Api.Services.Quiz;

namespace VocabTrainer.Api.Controllers
{
    [ApiController]
    [Route("api/quiz")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quiz;
        private readonly ICurrentUserAccessor _current;

        public QuizController(IQuizService quiz, ICurrentUserAccessor current)
        {
            _quiz = quiz;
            _current = current;
        }

        [HttpPost("sessions")]
        public async Task<IActionResult> Create([FromBody] CreateQuizSessionRequest req)
        {
            var res = await _quiz.CreateSession(_current.UserId, req);
            return res.Succeeded
                ? Ok(new { sessionId = res.Value })
                : res.ToActionResult();
        }

        public record NextQuizRequest(Guid SessionId);

        [HttpPost("sessions/next")]
        public async Task<IActionResult> Next([FromBody] NextQuizRequest req)
            => (await _quiz.Next(_current.UserId, req.SessionId)).ToActionResult();

        public record AnswerQuizBodyRequest(
            Guid SessionId,
            Guid WordId,
            bool IsCorrect,
            int ResponseTimeMs,
            string? SelectedAnswer
        );

        [HttpPost("sessions/answer")]
        public async Task<IActionResult> Answer([FromBody] AnswerQuizBodyRequest req)
        {
            var answerReq = new AnswerQuizRequest(
                req.WordId,
                req.SelectedAnswer,
                req.ResponseTimeMs
            );

            return (await _quiz.Answer(_current.UserId, req.SessionId, answerReq)).ToActionResult();
        }



        [HttpPost("sessions/summary")]
        public async Task<IActionResult> Summary([FromBody] SessionBodyRequest req)
            => (await _quiz.Summary(_current.UserId, req.SessionId)).ToActionResult();

        [HttpPost("sessions/close")]
        public async Task<IActionResult> Close([FromBody] SessionBodyRequest req)
            => (await _quiz.CloseSession(_current.UserId, req.SessionId)).ToActionResult();
    }
}
