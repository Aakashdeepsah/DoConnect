// AuthServiceTests.cs + QuestionServiceTests.cs + AdminServiceTests.cs
using DoConnect.API.DTOs;
using DoConnect.API.Helpers;
using DoConnect.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace DoConnect.Tests.Services
{
    // ===========================
    // Auth Tests
    // ===========================
    public class AuthServiceTests
    {
        private JwtHelper MakeJwt()
        {
            var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "TestSecret_AtLeast32CharactersLong!!",
                ["JwtSettings:Issuer"] = "Test", ["JwtSettings:Audience"] = "Test",
                ["JwtSettings:ExpiresInHours"] = "1"
            }).Build();
            return new JwtHelper(cfg);
        }

        [Fact]
        public async Task Register_ValidData_Succeeds()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            var result = await new AuthService(ctx, MakeJwt()).RegisterAsync(new RegisterDto
                { Username = "newuser", Email = "new@test.com", Password = "Pass123" });
            Assert.NotNull(result);
            Assert.Equal("newuser", result!.Username);
        }

        // FIX TEST: Registration ALWAYS creates User role, never Admin
        [Fact]
        public async Task Register_AlwaysCreatesUserRole_EvenIfAdminRequested()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            var result = await new AuthService(ctx, MakeJwt()).RegisterAsync(new RegisterDto
                { Username = "tryadmin", Email = "try@test.com", Password = "Pass123", Role = "Admin" });
            Assert.NotNull(result);
            // Must be "User" — the Role field in dto is ignored
            Assert.Equal("User", result!.Role);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsNull()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new AuthService(ctx, MakeJwt()).RegisterAsync(new RegisterDto
                { Username = "other", Email = "user@test.com", Password = "Pass123" });
            Assert.Null(result);
        }

        [Fact]
        public async Task Login_CorrectCredentials_ReturnsToken()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new AuthService(ctx, MakeJwt()).LoginAsync(new LoginDto
                { Email = "user@test.com", Password = "User@123" });
            Assert.NotNull(result);
            Assert.NotEmpty(result!.Token);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsNull()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new AuthService(ctx, MakeJwt()).LoginAsync(new LoginDto
                { Email = "user@test.com", Password = "WRONG" });
            Assert.Null(result);
        }
    }

    // ===========================
    // Question Tests
    // ===========================
    public class QuestionServiceTests
    {
        private ImageHelper MakeImageHelper()
        {
            var env = new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                { ["FileUpload:MaxFileSizeBytes"] = "5242880" }).Build();
            return new ImageHelper(env.Object, cfg);
        }

        [Fact]
        public async Task GetApproved_ReturnsOnlyApproved()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new QuestionService(ctx, MakeImageHelper()).GetApprovedQuestionsAsync();
            Assert.Single(result);
            Assert.Equal("Approved", result[0].Status);
        }

        // FIX TEST: GetById returns null for Pending questions (public cannot access)
        [Fact]
        public async Task GetById_PendingQuestion_ReturnsNull()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            // QuestionId 2 is Pending
            var result = await new QuestionService(ctx, MakeImageHelper()).GetQuestionByIdAsync(2);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetById_ApprovedQuestion_ReturnsQuestion()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new QuestionService(ctx, MakeImageHelper()).GetQuestionByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("Angular", result!.Topic);
        }

        [Fact]
        public async Task CreateQuestion_StatusIsPending()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new QuestionService(ctx, MakeImageHelper()).CreateQuestionAsync(
                new CreateQuestionDto { Topic = "C#", Title = "What is a delegate?",
                    QuestionText = "Help me understand delegates in C#." }, 2, "http://localhost");
            Assert.Equal("Pending", result.Status);
        }
    }

    // ===========================
    // Answer Tests
    // ===========================
    public class AnswerServiceTests
    {
        private ImageHelper MakeImageHelper()
        {
            var env = new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                { ["FileUpload:MaxFileSizeBytes"] = "5242880" }).Build();
            return new ImageHelper(env.Object, cfg);
        }

        // FIX TEST: Cannot answer a pending question
        [Fact]
        public async Task CreateAnswer_PendingQuestion_ThrowsException()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            // QuestionId 2 is Pending
            var svc = new AnswerService(ctx, MakeImageHelper());
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.CreateAnswerAsync(new CreateAnswerDto
                    { QuestionId = 2, AnswerText = "Trying to answer pending question." },
                    userId: 1, baseUrl: "http://localhost"));
        }

        [Fact]
        public async Task CreateAnswer_ApprovedQuestion_Succeeds()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new AnswerService(ctx, MakeImageHelper()).CreateAnswerAsync(
                new CreateAnswerDto { QuestionId = 1, AnswerText = "Here is my detailed answer." },
                userId: 2, baseUrl: "http://localhost");
            Assert.NotNull(result);
            Assert.Equal("Pending", result.Status); // Answers also need admin approval
        }
    }

    // ===========================
    // Admin Tests
    // ===========================
    public class AdminServiceTests
    {
        private ImageHelper MakeImageHelper()
        {
            var env = new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
            var cfg = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                { ["FileUpload:MaxFileSizeBytes"] = "5242880" }).Build();
            return new ImageHelper(env.Object, cfg);
        }

        [Fact]
        public async Task GetAllQuestions_ReturnsAll()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var result = await new AdminService(ctx, MakeImageHelper()).GetAllQuestionsAsync();
            Assert.Equal(2, result.Count); // Pending + Approved
        }

        [Fact]
        public async Task ApproveQuestion_ChangesStatus()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var ok = await new AdminService(ctx, MakeImageHelper()).UpdateQuestionStatusAsync(2, "Approved");
            Assert.True(ok);
            Assert.Equal("Approved", ctx.Questions.Find(2)!.Status);
        }

        // FIX TEST: Deleting a question with answers should succeed (cascade)
        [Fact]
        public async Task DeleteQuestion_WithAnswers_Succeeds()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            // Question 1 has 1 answer — cascade should handle it
            var ok = await new AdminService(ctx, MakeImageHelper()).DeleteQuestionAsync(1);
            Assert.True(ok);
            Assert.Null(ctx.Questions.Find(1));
        }

        [Fact]
        public async Task PendingCount_IsCorrect()
        {
            using var ctx = TestHelper.GetInMemoryDbContext();
            await TestHelper.SeedAsync(ctx);
            var count = await new AdminService(ctx, MakeImageHelper()).GetPendingCountAsync();
            Assert.Equal(1, count); // 1 pending question, 0 pending answers
        }
    }
}
