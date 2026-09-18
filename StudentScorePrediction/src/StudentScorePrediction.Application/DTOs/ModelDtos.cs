namespace StudentScorePrediction.Application.DTOs;

public class ModelVersionDto
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public string? ModelPath { get; set; }
    public int DatasetSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public ModelMetricsDto? Metrics { get; set; }
}

public class ModelMetricsDto
{
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
    public double MeanAbsolutePercentageError { get; set; }
}

public class TrainingRunDto
{
    public int Id { get; set; }
    public string Algorithm { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int DatasetSize { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int DurationSeconds { get; set; }
    public string? ErrorMessage { get; set; }
    public int? ModelVersionId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TrainingRequestDto
{
    public string Algorithm { get; set; } = "FastTreeRegression";
    public string? RequestedBy { get; set; }
}

public class DatasetStatisticsDto
{
    public int RecordCount { get; set; }
    public int FeatureCount { get; set; }
    public int MissingValues { get; set; }
    public bool HasOutliers { get; set; }
    public double MinFinalScore { get; set; }
    public double MaxFinalScore { get; set; }
    public double AverageFinalScore { get; set; }
    public Dictionary<string, FeatureStats>? FeatureStats { get; set; }
}

public class FeatureStats
{
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
    public double StdDev { get; set; }
}

public class DatasetInfoDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public int FeatureCount { get; set; }
    public int MissingValues { get; set; }
    public bool HasOutliers { get; set; }
    public double MinFinalScore { get; set; }
    public double MaxFinalScore { get; set; }
    public double AverageFinalScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DatasetStatisticsDto? Statistics { get; set; }
}

public class ModelComparisonDto
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public DateTime TrainingDate { get; set; }
    public int DatasetSize { get; set; }
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
    public bool IsActive { get; set; }
}

public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
