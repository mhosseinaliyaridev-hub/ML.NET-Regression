namespace StudentScorePrediction.Domain.Entities;

public class Prediction
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public float PredictedScore { get; set; }
    public float? ActualScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime PredictionTime { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
}
