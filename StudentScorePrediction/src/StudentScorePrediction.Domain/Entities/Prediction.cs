namespace StudentScorePrediction.Domain.Entities;

public class Prediction
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public double PredictedScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Student? Student { get; set; }
}
