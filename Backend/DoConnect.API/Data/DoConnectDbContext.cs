// Data/DoConnectDbContext.cs
using DoConnect.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DoConnect.API.Data
{
    public class DoConnectDbContext : DbContext
    {
        public DoConnectDbContext(DbContextOptions<DoConnectDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserId);
                e.Property(u => u.Username).IsRequired().HasMaxLength(100);
                e.Property(u => u.Email).IsRequired().HasMaxLength(200);
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
                e.Property(u => u.Role).IsRequired().HasMaxLength(20).HasDefaultValue("User");
                e.HasIndex(u => u.Username).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Question>(e =>
            {
                e.HasKey(q => q.QuestionId);
                e.Property(q => q.Topic).IsRequired().HasMaxLength(100);
                e.Property(q => q.Title).IsRequired().HasMaxLength(300);
                e.Property(q => q.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
                e.HasOne(q => q.User)
                 .WithMany(u => u.Questions)
                 .HasForeignKey(q => q.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Answer>(e =>
            {
                e.HasKey(a => a.AnswerId);
                e.Property(a => a.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
                // FIX: Cascade delete — removing a question removes its answers
                e.HasOne(a => a.Question)
                 .WithMany(q => q.Answers)
                 .HasForeignKey(a => a.QuestionId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(a => a.User)
                 .WithMany(u => u.Answers)
                 .HasForeignKey(a => a.UserId)
                 .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
