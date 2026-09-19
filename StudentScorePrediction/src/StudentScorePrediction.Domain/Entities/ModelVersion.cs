using StudentScorePrediction.Domain.Enums;

namespace StudentScorePrediction.Domain.Entities;

public class ModelVersion
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public AlgorithmType Algorithm { get; set; }
    public string ModelPath { get; set; } = string.Empty;
    public float MAE { get; set; }
    public float MSE { get; set; }
    public float RMSE { get; set; }
    public float RSquared { get; set; }
    public int DatasetSize { get; set; }
    public TimeSpan TrainingDuration { get; set; }
    public DateTime TrainingDate { get; set; }
    public ModelStatus Status { get; set; } = ModelStatus.Inactive;
    public bool IsActive { get; set; }

    public ICollection<TrainingRun>? TrainingRuns { get; set; }
}
