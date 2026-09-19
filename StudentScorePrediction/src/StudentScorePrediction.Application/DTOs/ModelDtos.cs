using StudentScorePrediction.Domain.Enums;

namespace StudentScorePrediction.Application.DTOs;

public class ModelVersionDto
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public AlgorithmType Algorithm { get; set; }
    public float MAE { get; set; }
    public float MSE { get; set; }
    public float RMSE { get; set; }
    public float RSquared { get; set; }
    public int DatasetSize { get; set; }
    public TimeSpan TrainingDuration { get; set; }
    public DateTime TrainingDate { get; set; }
    public bool IsActive { get; set; }
}

public class TrainingRunDto
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
}

public class TrainingRequestDto
{
    public string DatasetPath { get; set; } = string.Empty;
    public AlgorithmType Algorithm { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
}

public class ModelEvaluationDto
{
    public AlgorithmType Algorithm { get; set; }
    public float MAE { get; set; }
    public float MSE { get; set; }
    public float RMSE { get; set; }
    public float RSquared { get; set; }
    public TimeSpan TrainingDuration { get; set; }
    public string? Error { get; set; }
}
