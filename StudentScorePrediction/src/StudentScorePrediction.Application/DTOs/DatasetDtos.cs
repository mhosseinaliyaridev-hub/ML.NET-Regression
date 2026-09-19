namespace StudentScorePrediction.Application.DTOs;

public class DatasetDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsGenerated { get; set; }
}

public class DatasetStatisticsDto
{
    public int TotalRecords { get; set; }
    public int FeatureCount { get; set; }
    public int MissingValues { get; set; }
    public Dictionary<string, float> MinValues { get; set; } = new();
    public Dictionary<string, float> MaxValues { get; set; } = new();
    public Dictionary<string, float> AverageValues { get; set; } = new();
    public List<string> Outliers { get; set; } = new();
}

public class GenerateDatasetRequestDto
{
    public int RecordCount { get; set; } = 100000;
}
