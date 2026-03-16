// Models/Question.cs
namespace DoConnect.API.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
