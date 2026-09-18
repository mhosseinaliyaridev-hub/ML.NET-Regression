using StudentScorePrediction.Domain.Enums;

namespace StudentScorePrediction.Domain.Entities;

public class TrainingRun
{
    public int Id { get; set; }
    public int ModelVersionId { get; set; }
    public AlgorithmType Algorithm { get; set; }
    public string DatasetPath { get; set; } = string.Empty;
    public int DatasetSize { get; set; }
    public TrainingStatus Status { get; set; }
    public float MAE { get; set; }
    public float MSE { get; set; }
    public float RMSE { get; set; }
    public float RSquared { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? ErrorMessage { get; set; }

    public ModelVersion? ModelVersion { get; set; }
}
