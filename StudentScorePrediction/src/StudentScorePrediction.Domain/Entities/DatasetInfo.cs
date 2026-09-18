namespace StudentScorePrediction.Domain.Entities;

public class DatasetInfo
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsGenerated { get; set; }
    public string? Description { get; set; }
}
