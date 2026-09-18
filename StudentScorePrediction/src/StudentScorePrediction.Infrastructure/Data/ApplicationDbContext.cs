using Microsoft.EntityFrameworkCore;
using StudentScorePrediction.Domain.Entities;

namespace StudentScorePrediction.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<ModelVersion> ModelVersions => Set<ModelVersion>();
    public DbSet<TrainingRun> TrainingRuns => Set<TrainingRun>();
    public DbSet<DatasetInfo> DatasetInfos => Set<DatasetInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Gender).HasMaxLength(10).IsRequired();
            entity.HasIndex(e => e.LastName);
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ModelVersion).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Algorithm).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Predictions)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<ModelVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Algorithm).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<TrainingRun>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Algorithm).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartedAt);
        });

        modelBuilder.Entity<DatasetInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
        });
    }
}
