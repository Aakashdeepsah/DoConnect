// Interfaces/IServices.cs
using DoConnect.API.DTOs;
namespace DoConnect.API.Interfaces
{
    public interface IQuestionService
    {
        Task<List<QuestionResponseDto>> GetApprovedQuestionsAsync();
        Task<List<QuestionResponseDto>> SearchQuestionsAsync(string query);
        Task<QuestionResponseDto?> GetQuestionByIdAsync(int id);
        Task<QuestionResponseDto> CreateQuestionAsync(CreateQuestionDto dto, int userId, string baseUrl);
        Task<List<QuestionResponseDto>> GetQuestionsByUserAsync(int userId);
    }

    public interface IAnswerService
    {
        Task<List<AnswerResponseDto>> GetApprovedAnswersAsync(int questionId);
        Task<AnswerResponseDto> CreateAnswerAsync(CreateAnswerDto dto, int userId, string baseUrl);
    }

    public interface IAdminService
    {
        Task<List<QuestionResponseDto>> GetAllQuestionsAsync();
        Task<List<AnswerResponseDto>> GetAllAnswersAsync();
        Task<bool> UpdateQuestionStatusAsync(int questionId, string status);
        Task<bool> UpdateAnswerStatusAsync(int answerId, string status);
        Task<bool> DeleteQuestionAsync(int questionId);
        Task<bool> DeleteAnswerAsync(int answerId);
        Task<int> GetPendingCountAsync();
    }
}
