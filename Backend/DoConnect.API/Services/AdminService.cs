// Services/AdminService.cs
// FIX: Question deletion now works because the DB schema has ON DELETE CASCADE.
//      EF Core removes the question; SQL Server auto-removes its answers.
using DoConnect.API.Data;
using DoConnect.API.DTOs;
using DoConnect.API.Helpers;
using DoConnect.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoConnect.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly DoConnectDbContext _context;
        private readonly ImageHelper _imageHelper;

        public AdminService(DoConnectDbContext context, ImageHelper imageHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
        }

        public async Task<List<QuestionResponseDto>> GetAllQuestionsAsync()
        {
            var questions = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return questions.Select(q => new QuestionResponseDto
            {
                QuestionId   = q.QuestionId,
                UserId       = q.UserId,
                Username     = q.User?.Username ?? "Unknown",
                Topic        = q.Topic,
                Title        = q.Title,
                QuestionText = q.QuestionText,
                Status       = q.Status,
                ImageUrl     = ImageHelper.BuildImageUrl(q.ImagePath, ""),
                CreatedAt    = q.CreatedAt,
                AnswerCount  = q.Answers?.Count ?? 0
            }).ToList();
        }

        public async Task<List<AnswerResponseDto>> GetAllAnswersAsync()
        {
            var answers = await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return answers.Select(a => new AnswerResponseDto
            {
                AnswerId   = a.AnswerId,
                QuestionId = a.QuestionId,
                UserId     = a.UserId,
                Username   = a.User?.Username ?? "Unknown",
                AnswerText = a.AnswerText,
                Status     = a.Status,
                ImageUrl   = ImageHelper.BuildImageUrl(a.ImagePath, ""),
                CreatedAt  = a.CreatedAt
            }).ToList();
        }

        public async Task<bool> UpdateQuestionStatusAsync(int questionId, string status)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null) return false;
            question.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAnswerStatusAsync(int answerId, string status)
        {
            var answer = await _context.Answers.FindAsync(answerId);
            if (answer == null) return false;
            answer.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        // FIX: Deletion now works because ON DELETE CASCADE in the schema
        // automatically removes all answers when the question is deleted.
        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null) return false;

            // Delete image files for the question
            _imageHelper.DeleteImage(question.ImagePath);

            // Also clean up answer images before cascade removes the records
            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
            foreach (var answer in answers)
                _imageHelper.DeleteImage(answer.ImagePath);

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAnswerAsync(int answerId)
        {
            var answer = await _context.Answers.FindAsync(answerId);
            if (answer == null) return false;
            _imageHelper.DeleteImage(answer.ImagePath);
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetPendingCountAsync()
        {
            var pq = await _context.Questions.CountAsync(q => q.Status == "Pending");
            var pa = await _context.Answers.CountAsync(a => a.Status == "Pending");
            return pq + pa;
        }
    }
}
