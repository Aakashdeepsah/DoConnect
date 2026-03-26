// Models/Answer.cs — Sprint 2: added UpdatedAt
namespace DoConnect.API.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Sprint 2: tracks when admin last updated the status
        public DateTime? UpdatedAt { get; set; }

        public Question? Question { get; set; }
        public User? User { get; set; }
    }
}
