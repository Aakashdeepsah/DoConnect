// DTOs/Dtos.cs — Sprint 2 additions
// Changes:
//   1. QuestionResponseDto and AnswerResponseDto get UpdatedAt field
//   2. RegisterDto gets ConfirmPassword with matching validation
//   3. New UserSummaryDto for admin user list
//   4. New ChangePasswordDto for future use
using System.ComponentModel.DataAnnotations;

namespace DoConnect.API.DTOs
{
    // ---- Registration — Sprint 2: added ConfirmPassword ----
    public class RegisterDto : IValidatableObject
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

        // Sprint 2: confirm password field
        [Required(ErrorMessage = "Please confirm your password.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        // Custom cross-field validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != ConfirmPassword)
            {
                yield return new ValidationResult(
                    "Password and Confirm Password do not match.",
                    new[] { nameof(ConfirmPassword) });
            }
        }
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

    // ---- Auth response ----
    public class AuthResponseDto
    {
        public string Token    { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role     { get; set; } = string.Empty;
        public int    UserId   { get; set; }
        // Sprint 2: tell the frontend when the token expires
        public DateTime ExpiresAt { get; set; }
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

    // ---- Question response — Sprint 2: added UpdatedAt ----
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
        public DateTime? UpdatedAt { get; set; } // Sprint 2: when admin last acted
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

    // ---- Answer response — Sprint 2: added UpdatedAt ----
    public class AnswerResponseDto
    {
        public int    AnswerId   { get; set; }
        public int    QuestionId { get; set; }
        public int    UserId     { get; set; }
        public string Username   { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        public string Status     { get; set; } = string.Empty;
        public string? ImageUrl  { get; set; }
        public DateTime CreatedAt  { get; set; }
        public DateTime? UpdatedAt { get; set; } // Sprint 2
    }

    // ---- Admin status update ----
    public class UpdateStatusDto
    {
        [Required]
        [RegularExpression("^(Approved|Rejected)$",
            ErrorMessage = "Status must be 'Approved' or 'Rejected'.")]
        public string Status { get; set; } = string.Empty;
    }

    // ---- Sprint 2: User summary for admin user list ----
    public class UserSummaryDto
    {
        public int    UserId        { get; set; }
        public string Username      { get; set; } = string.Empty;
        public string Email         { get; set; } = string.Empty;
        public string Role          { get; set; } = string.Empty;
        public DateTime CreatedAt   { get; set; }
        public int    QuestionCount { get; set; }
        public int    AnswerCount   { get; set; }
    }

    // ---- Sprint 2: Change password ----
    public class ChangePasswordDto : IValidatableObject
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmNewPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewPassword != ConfirmNewPassword)
                yield return new ValidationResult("Passwords do not match.",
                    new[] { nameof(ConfirmNewPassword) });
        }
    }
}
