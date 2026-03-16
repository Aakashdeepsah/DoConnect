// Tests - TestHelper.cs
using DoConnect.API.Data;
using DoConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoConnect.Tests
{
    public static class TestHelper
    {
        public static DoConnectDbContext GetInMemoryDbContext(string? name = null)
        {
            var options = new DbContextOptionsBuilder<DoConnectDbContext>()
                .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
                .Options;
            return new DoConnectDbContext(options);
        }

        public static async Task SeedAsync(DoConnectDbContext ctx)
        {
            var admin = new User { UserId = 1, Username = "admin",    Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = "Admin" };
            var user  = new User { UserId = 2, Username = "testuser", Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),  Role = "User" };
            ctx.Users.AddRange(admin, user);

            var approvedQ = new Question { QuestionId = 1, UserId = 2, Topic = "Angular",
                Title = "How do Angular services work?",
                QuestionText = "I need help understanding Angular services.",
                Status = "Approved" };
            var pendingQ = new Question { QuestionId = 2, UserId = 2, Topic = ".NET",
                Title = "How does EF Core work?",
                QuestionText = "What is Entity Framework Core?",
                Status = "Pending" };
            ctx.Questions.AddRange(approvedQ, pendingQ);

            ctx.Answers.Add(new Answer { AnswerId = 1, QuestionId = 1, UserId = 1,
                AnswerText = "Services are singleton classes for sharing data.", Status = "Approved" });

            await ctx.SaveChangesAsync();
        }
    }
}
