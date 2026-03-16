// Models/User.cs
namespace DoConnect.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
