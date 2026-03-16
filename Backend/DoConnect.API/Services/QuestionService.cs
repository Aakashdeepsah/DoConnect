// Services/QuestionService.cs
// FIX 1: GetQuestionByIdAsync now only returns Approved questions for public access
// FIX 2: Search uses direct Contains (no ToLower) — DB collation handles case
using DoConnect.API.Data;
using DoConnect.API.DTOs;
using DoConnect.API.Helpers;
using DoConnect.API.Interfaces;
using DoConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoConnect.API.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly DoConnectDbContext _context;
        private readonly ImageHelper _imageHelper;

        public QuestionService(DoConnectDbContext context, ImageHelper imageHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
        }

        public async Task<List<QuestionResponseDto>> GetApprovedQuestionsAsync()
        {
            var questions = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .Where(q => q.Status == "Approved")
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return questions.Select(q => MapToDto(q, "")).ToList();
        }

        // FIX: Use EF.Functions.Like or plain Contains — do not use ToLower()
        // The SQL Server default collation is case-insensitive already.
        public async Task<List<QuestionResponseDto>> SearchQuestionsAsync(string searchQuery)
        {
            var query = searchQuery.Trim();

            var questions = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .Where(q =>
                    q.Status == "Approved" &&
                    (q.Title.Contains(query) ||
                     q.QuestionText.Contains(query) ||
                     q.Topic.Contains(query)))
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return questions.Select(q => MapToDto(q, "")).ToList();
        }

        // FIX: Public endpoint only returns Approved questions.
        // Pending/Rejected are hidden from public users even if they know the ID.
        // Admins use a separate endpoint (AdminController) to see all statuses.
        public async Task<QuestionResponseDto?> GetQuestionByIdAsync(int questionId)
        {
            var question = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q =>
                    q.QuestionId == questionId &&
                    q.Status == "Approved");   // Only approved!

            if (question == null) return null;
            return MapToDto(question, "");
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(
            CreateQuestionDto dto, int userId, string baseUrl)
        {
            var imagePath = await _imageHelper.SaveImageAsync(dto.Image);

            var question = new Question
            {
                UserId       = userId,
                Topic        = dto.Topic.Trim(),
                Title        = dto.Title.Trim(),
                QuestionText = dto.QuestionText.Trim(),
                Status       = "Pending",
                ImagePath    = imagePath,
                CreatedAt    = DateTime.UtcNow
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            await _context.Entry(question).Reference(q => q.User).LoadAsync();
            return MapToDto(question, baseUrl);
        }

        public async Task<List<QuestionResponseDto>> GetQuestionsByUserAsync(int userId)
        {
            var questions = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return questions.Select(q => MapToDto(q, "")).ToList();
        }

        private QuestionResponseDto MapToDto(Question q, string baseUrl)
        {
            return new QuestionResponseDto
            {
                QuestionId   = q.QuestionId,
                UserId       = q.UserId,
                Username     = q.User?.Username ?? "Unknown",
                Topic        = q.Topic,
                Title        = q.Title,
                QuestionText = q.QuestionText,
                Status       = q.Status,
                ImageUrl     = ImageHelper.BuildImageUrl(q.ImagePath, baseUrl),
                CreatedAt    = q.CreatedAt,
                AnswerCount  = q.Answers?.Count(a => a.Status == "Approved") ?? 0
            };
        }
    }
}
