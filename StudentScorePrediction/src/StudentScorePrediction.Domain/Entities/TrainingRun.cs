namespace StudentScorePrediction.Domain.Entities;

public class TrainingRun
{
    public int Id { get; set; }
    public string Algorithm { get; set; } = string.Empty;
    public int DatasetSize { get; set; }
    public string Status { get; set; } = "Queued"; // Queued, Loading Data, Preparing Data, Training, Evaluating, Saving Model, Completed, Failed
    public double? MAE { get; set; }
    public double? MSE { get; set; }
    public double? RMSE { get; set; }
    public double? RSquared { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
