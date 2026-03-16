// Controllers/QuestionController.cs
using DoConnect.API.DTOs;
using DoConnect.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        public QuestionController(IQuestionService s) { _questionService = s; }

        [HttpGet]
        public async Task<IActionResult> GetApproved() =>
            Ok(await _questionService.GetApprovedQuestionsAsync());

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Search query cannot be empty." });
            return Ok(await _questionService.SearchQuestionsAsync(query));
        }

        // FIX: Only returns Approved questions. Pending/Rejected return 404.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var q = await _questionService.GetQuestionByIdAsync(id);
            if (q == null) return NotFound(new { message = "Question not found or not yet approved." });
            return Ok(q);
        }

        [HttpGet("my-questions")]
        [Authorize]
        public async Task<IActionResult> GetMine()
        {
            var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return Ok(await _questionService.GetQuestionsByUserAsync(uid));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });

            var uid     = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result  = await _questionService.CreateQuestionAsync(dto, uid, baseUrl);
            return CreatedAtAction(nameof(GetById), new { id = result.QuestionId }, result);
        }
    }
}
