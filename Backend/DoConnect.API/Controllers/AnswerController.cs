// Controllers/AnswerController.cs
using DoConnect.API.DTOs;
using DoConnect.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        public AnswerController(IAnswerService s) { _answerService = s; }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetAnswers(int questionId) =>
            Ok(await _answerService.GetApprovedAnswersAsync(questionId));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateAnswerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });

            var uid     = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result  = await _answerService.CreateAnswerAsync(dto, uid, baseUrl);
            return Ok(result);
        }
    }
}
