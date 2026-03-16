// Services/AnswerService.cs
// FIX: Prevents answering unapproved (Pending/Rejected) questions
using DoConnect.API.Data;
using DoConnect.API.DTOs;
using DoConnect.API.Helpers;
using DoConnect.API.Interfaces;
using DoConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoConnect.API.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly DoConnectDbContext _context;
        private readonly ImageHelper _imageHelper;

        public AnswerService(DoConnectDbContext context, ImageHelper imageHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
        }

        public async Task<List<AnswerResponseDto>> GetApprovedAnswersAsync(int questionId)
        {
            var answers = await _context.Answers
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId && a.Status == "Approved")
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();

            return answers.Select(a => MapToDto(a, "")).ToList();
        }

        public async Task<AnswerResponseDto> CreateAnswerAsync(
            CreateAnswerDto dto, int userId, string baseUrl)
        {
            // FIX: Check the question exists AND is Approved.
            // Users cannot answer Pending or Rejected questions.
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.QuestionId == dto.QuestionId);

            if (question == null)
                throw new KeyNotFoundException($"Question with ID {dto.QuestionId} not found.");

            if (question.Status != "Approved")
                throw new InvalidOperationException(
                    "You can only answer questions that have been approved.");

            var imagePath = await _imageHelper.SaveImageAsync(dto.Image);

            var answer = new Answer
            {
                QuestionId = dto.QuestionId,
                UserId     = userId,
                AnswerText = dto.AnswerText.Trim(),
                Status     = "Pending",
                ImagePath  = imagePath,
                CreatedAt  = DateTime.UtcNow
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            await _context.Entry(answer).Reference(a => a.User).LoadAsync();
            return MapToDto(answer, baseUrl);
        }

        private AnswerResponseDto MapToDto(Answer a, string baseUrl) => new()
        {
            AnswerId   = a.AnswerId,
            QuestionId = a.QuestionId,
            UserId     = a.UserId,
            Username   = a.User?.Username ?? "Unknown",
            AnswerText = a.AnswerText,
            Status     = a.Status,
            ImageUrl   = ImageHelper.BuildImageUrl(a.ImagePath, baseUrl),
            CreatedAt  = a.CreatedAt
        };
    }
}
