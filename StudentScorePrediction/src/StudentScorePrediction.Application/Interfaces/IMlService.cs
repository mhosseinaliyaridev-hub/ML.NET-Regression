using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Domain.Enums;

namespace StudentScorePrediction.Application.Interfaces;

public interface IMlService
{
    Task<string> GenerateDatasetAsync(int recordCount, CancellationToken cancellationToken = default);
    Task<TrainingResult> TrainModelAsync(string datasetPath, AlgorithmType algorithm, string modelVersion, CancellationToken cancellationToken = default);
    PredictionResult Predict(StudentInput input);
    Task<PredictionResult> PredictWithModelAsync(string modelPath, StudentInput input, CancellationToken cancellationToken = default);
    Task<IEnumerable<ModelEvaluation>> CompareAlgorithmsAsync(string datasetPath, CancellationToken cancellationToken = default);
    Task LoadModelAsync(string modelPath, string version, CancellationToken cancellationToken = default);
    Task<DatasetStatisticsDto> GetDatasetStatisticsAsync(string filePath, CancellationToken cancellationToken = default);
}

public class TrainingResult
{
    public string ModelPath { get; set; } = string.Empty;
    public string ModelVersion { get; set; } = string.Empty;
    public AlgorithmType Algorithm { get; set; }
    public RegressionMetrics Metrics { get; set; } = new();
    public TimeSpan TrainingDuration { get; set; }
    public int DatasetSize { get; set; }
}

public class RegressionMetrics
{
    public float MeanAbsoluteError { get; set; }
    public float MeanSquaredError { get; set; }
    public float RootMeanSquaredError { get; set; }
    public float RSquared { get; set; }
}

public class PredictionResult
{
    public float PredictedScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public DateTime PredictionTime { get; set; }
    public TimeSpan Duration { get; set; }
}

public class StudentInput
{
    public float Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public float StudyHours { get; set; }
    public float AttendanceRate { get; set; }
    public float HomeworkCompletionRate { get; set; }
    public float PreviousAverage { get; set; }
    public float PreviousExamScore { get; set; }
    public float MidtermScore { get; set; }
    public float AbsenceDays { get; set; }
    public float SleepHours { get; set; }
    public float ClassParticipation { get; set; }
    public float MobileUsageHours { get; set; }
    public float PracticeTestCount { get; set; }
}

public class PredictionOutput
{
    public float FinalScore { get; set; }
}

public class ModelEvaluation
{
    public AlgorithmType Algorithm { get; set; }
    public float MAE { get; set; }
    public float MSE { get; set; }
    public float RMSE { get; set; }
    public float RSquared { get; set; }
    public TimeSpan TrainingDuration { get; set; }
    public string? Error { get; set; }
}
