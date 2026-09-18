namespace StudentScorePrediction.Application.DTOs;

public class PredictionDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public float PredictedScore { get; set; }
    public float? ActualScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime PredictionTime { get; set; }
}

public class CreatePredictionDto
{
    public int StudentId { get; set; }
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

public class PredictionResultDto
{
    public float PredictedScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public DateTime PredictionTime { get; set; }
    public TimeSpan Duration { get; set; }
}
