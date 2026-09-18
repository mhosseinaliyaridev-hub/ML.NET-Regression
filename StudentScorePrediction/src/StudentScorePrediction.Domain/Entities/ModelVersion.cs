namespace StudentScorePrediction.Domain.Entities;

public class ModelVersion
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
    public int DatasetSize { get; set; }
    public TimeSpan TrainingDuration { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
    public string? ModelPath { get; set; }
}
