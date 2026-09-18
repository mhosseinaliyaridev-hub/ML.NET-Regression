namespace StudentScorePrediction.Application.DTOs;

public class PredictionDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public double PredictedScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePredictionDto
{
    public int StudentId { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public double StudyHours { get; set; }
    public double AttendanceRate { get; set; }
    public double HomeworkCompletionRate { get; set; }
    public double PreviousAverage { get; set; }
    public double PreviousExamScore { get; set; }
    public double MidtermScore { get; set; }
    public int AbsenceDays { get; set; }
    public double SleepHours { get; set; }
    public double ClassParticipation { get; set; }
    public double MobileUsageHours { get; set; }
    public int PracticeTestCount { get; set; }
}

public class PredictionResultDto
{
    public double PredictedScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public Dictionary<string, double>? FeatureImportance { get; set; }
}
