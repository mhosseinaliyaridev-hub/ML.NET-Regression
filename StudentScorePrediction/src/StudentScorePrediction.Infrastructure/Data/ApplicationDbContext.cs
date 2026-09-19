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

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Age).IsRequired();
            entity.Property(e => e.Gender).HasMaxLength(10).IsRequired();
            entity.Property(e => e.StudyHours).IsRequired();
            entity.Property(e => e.AttendanceRate).IsRequired();
            entity.Property(e => e.HomeworkCompletionRate).IsRequired();
            entity.Property(e => e.PreviousAverage).IsRequired();
            entity.Property(e => e.PreviousExamScore).IsRequired();
            entity.Property(e => e.MidtermScore).IsRequired();
            entity.Property(e => e.AbsenceDays).IsRequired();
            entity.Property(e => e.SleepHours).IsRequired();
            entity.Property(e => e.ClassParticipation).IsRequired();
            entity.Property(e => e.MobileUsageHours).IsRequired();
            entity.Property(e => e.PracticeTestCount).IsRequired();
            
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Age);
        });

        // Prediction configuration
        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PredictedScore).IsRequired();
            entity.Property(e => e.ActualScore);
            entity.Property(e => e.ModelVersionId).IsRequired();
            entity.Property(e => e.PredictionDate).IsRequired();
            entity.Property(e => e.PredictionDurationMs).IsRequired();
            
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Predictions)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.ModelVersion)
                  .WithMany(m => m.Predictions)
                  .HasForeignKey(e => e.ModelVersionId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.ModelVersionId);
            entity.HasIndex(e => e.PredictionDate);
        });

        // ModelVersion configuration
        modelBuilder.Entity<ModelVersion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Algorithm).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TrainingDate).IsRequired();
            entity.Property(e => e.DatasetSize).IsRequired();
            entity.Property(e => e.MAE).IsRequired();
            entity.Property(e => e.MSE).IsRequired();
            entity.Property(e => e.RMSE).IsRequired();
            entity.Property(e => e.RSquared).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.ModelPath).HasMaxLength(500);
            entity.Property(e => e.TrainingDurationSeconds).IsRequired();
            
            entity.HasIndex(e => e.Version);
            entity.HasIndex(e => e.IsActive);
        });

        // TrainingRun configuration
        modelBuilder.Entity<TrainingRun>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime);
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DatasetSize).IsRequired();
            entity.Property(e => e.Algorithm).HasMaxLength(50).IsRequired();
            entity.Property(e => e.MAE);
            entity.Property(e => e.MSE);
            entity.Property(e => e.RMSE);
            entity.Property(e => e.RSquared);
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.Status);
        });

        // DatasetInfo configuration
        modelBuilder.Entity<DatasetInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RecordCount).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.Source).HasMaxLength(50).IsRequired();
            
            entity.HasIndex(e => e.CreatedDate);
        });
    }
}
