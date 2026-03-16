// DTOs/Dtos.cs
// FIX: Added data annotations for automatic model validation
using System.ComponentModel.DataAnnotations;

namespace DoConnect.API.DTOs
{
    // ---- Registration ----
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be 3-100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        // FIX: Role is intentionally ignored from client input.
        // Backend always sets this to "User". Admins are seeded in DB only.
        // This property is kept for DTO compatibility but not used in service.
        public string Role { get; set; } = "User";
    }

    // ---- Login ----
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    // ---- Auth response returned to frontend ----
    public class AuthResponseDto
    {
        public string Token    { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role     { get; set; } = string.Empty;
        public int    UserId   { get; set; }
    }

    // ---- Create question ----
    public class CreateQuestionDto
    {
        [Required(ErrorMessage = "Topic is required.")]
        [StringLength(100)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(300, MinimumLength = 10, ErrorMessage = "Title must be 10-300 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Question text is required.")]
        [MinLength(20, ErrorMessage = "Question must be at least 20 characters.")]
        public string QuestionText { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }

    // ---- Question response ----
    public class QuestionResponseDto
    {
        public int    QuestionId   { get; set; }
        public int    UserId       { get; set; }
        public string Username     { get; set; } = string.Empty;
        public string Topic        { get; set; } = string.Empty;
        public string Title        { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string Status       { get; set; } = string.Empty;
        public string? ImageUrl    { get; set; }
        public DateTime CreatedAt  { get; set; }
        public int    AnswerCount  { get; set; }
    }

    // ---- Create answer ----
    public class CreateAnswerDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A valid Question ID is required.")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Answer text is required.")]
        [MinLength(10, ErrorMessage = "Answer must be at least 10 characters.")]
        public string AnswerText { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }

    // ---- Answer response ----
    public class AnswerResponseDto
    {
        public int    AnswerId   { get; set; }
        public int    QuestionId { get; set; }
        public int    UserId     { get; set; }
        public string Username   { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        public string Status     { get; set; } = string.Empty;
        public string? ImageUrl  { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ---- Admin status update ----
    public class UpdateStatusDto
    {
        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be 'Approved' or 'Rejected'.")]
        public string Status { get; set; } = string.Empty;
    }
}
