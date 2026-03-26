// Controllers/AdminController.cs — Sprint 2: added GET /api/admin/users
using DoConnect.API.DTOs;
using DoConnect.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService s) { _adminService = s; }

        // ── Questions ──
        [HttpGet("questions")]
        public async Task<IActionResult> GetAllQuestions()
        {
            var questions = await _adminService.GetAllQuestionsAsync();
            var baseUrl   = $"{Request.Scheme}://{Request.Host}";
            foreach (var q in questions)
                if (!string.IsNullOrEmpty(q.ImageUrl) && !q.ImageUrl.StartsWith("http"))
                    q.ImageUrl = $"{baseUrl}/uploads/{q.ImageUrl}";
            return Ok(questions);
        }

        [HttpPut("questions/{id}/status")]
        public async Task<IActionResult> UpdateQuestionStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _adminService.UpdateQuestionStatusAsync(id, dto.Status))
                return NotFound(new { message = $"Question {id} not found." });
            return Ok(new { message = $"Question {dto.Status}." });
        }

        [HttpDelete("questions/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            if (!await _adminService.DeleteQuestionAsync(id))
                return NotFound(new { message = $"Question {id} not found." });
            return Ok(new { message = "Question deleted." });
        }

        // ── Answers ──
        [HttpGet("answers")]
        public async Task<IActionResult> GetAllAnswers()
        {
            var answers = await _adminService.GetAllAnswersAsync();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var a in answers)
                if (!string.IsNullOrEmpty(a.ImageUrl) && !a.ImageUrl.StartsWith("http"))
                    a.ImageUrl = $"{baseUrl}/uploads/{a.ImageUrl}";
            return Ok(answers);
        }

        [HttpPut("answers/{id}/status")]
        public async Task<IActionResult> UpdateAnswerStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _adminService.UpdateAnswerStatusAsync(id, dto.Status))
                return NotFound(new { message = $"Answer {id} not found." });
            return Ok(new { message = $"Answer {dto.Status}." });
        }

        [HttpDelete("answers/{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            if (!await _adminService.DeleteAnswerAsync(id))
                return NotFound(new { message = $"Answer {id} not found." });
            return Ok(new { message = "Answer deleted." });
        }

        // ── Pending count ──
        [HttpGet("pending-count")]
        public async Task<IActionResult> PendingCount() =>
            Ok(new { pendingCount = await _adminService.GetPendingCountAsync() });

        // ── Sprint 2: User list ──
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers() =>
            Ok(await _adminService.GetAllUsersAsync());
    }
}
