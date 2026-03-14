using Microsoft.AspNetCore.Mvc;

namespace VocabTrainer.Api.Abstractions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.Succeeded) return new OkResult();
            return new BadRequestObjectResult(result.Error);
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.Succeeded) return new OkObjectResult(result.Value);
            return new BadRequestObjectResult(result.Error);
        }
    }
}