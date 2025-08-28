using Microsoft.EntityFrameworkCore;
using LoanOrigination.Api.Models;

namespace LoanOrigination.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<LoanApplication> Applications => Set<LoanApplication>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<Prediction> Predictions => Set<Prediction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                
                // Default values
                entity.Property(e => e.Role).HasDefaultValue("User");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // LoanApplication configuration
            modelBuilder.Entity<LoanApplication>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Applications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ReviewedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ReviewedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Default values
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Document configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasOne(e => e.LoanApplication)
                    .WithMany(la => la.Documents)
                    .HasForeignKey(e => e.LoanApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.VerifiedBy)
                    .WithMany()
                    .HasForeignKey(e => e.VerifiedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Default values
                entity.Property(e => e.IsVerified).HasDefaultValue(false);
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes
                entity.HasIndex(e => e.LoanApplicationId);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.UploadedAt);
            });

            // Prediction configuration
            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.HasOne(e => e.LoanApplication)
                    .WithOne(la => la.Prediction)
                    .HasForeignKey<Prediction>(e => e.LoanApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Default values
                entity.Property(e => e.ModelVersion).HasDefaultValue("1.0");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes
                entity.HasIndex(e => e.LoanApplicationId).IsUnique();
                entity.HasIndex(e => e.CreatedAt);
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is User || e.Entity is LoanApplication || e.Entity is Prediction)
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is LoanApplication application)
                {
                    application.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is Prediction prediction)
                {
                    prediction.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
