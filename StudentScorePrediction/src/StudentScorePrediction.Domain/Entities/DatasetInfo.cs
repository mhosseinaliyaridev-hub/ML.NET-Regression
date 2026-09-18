namespace StudentScorePrediction.Domain.Entities;

public class DatasetInfo
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int MissingValues { get; set; }
    public int Outliers { get; set; }
}
