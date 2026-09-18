namespace StudentScorePrediction.Application.DTOs;

public class ModelVersionDto
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
    public int DatasetSize { get; set; }
    public long TrainingDurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class TrainingRunDto
{
    public int Id { get; set; }
    public string Algorithm { get; set; } = string.Empty;
    public int DatasetSize { get; set; }
    public string Status { get; set; } = string.Empty;
    public double? MAE { get; set; }
    public double? MSE { get; set; }
    public double? RMSE { get; set; }
    public double? RSquared { get; set; }
    public long? DurationMs { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class StartTrainingDto
{
    public string Algorithm { get; set; } = "FastTreeRegression";
}

public class DatasetStatisticsDto
{
    public int RecordCount { get; set; }
    public int MissingValues { get; set; }
    public int Outliers { get; set; }
    public Dictionary<string, FeatureStats>? FeatureStats { get; set; }
}

public class FeatureStats
{
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
}

public class GenerateDatasetDto
{
    public int RecordCount { get; set; } = 100000;
}
