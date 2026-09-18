namespace StudentScorePrediction.Application.DTOs;

public class PredictionDto
{
    public int Id { get; set; }
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
    public double PredictedScore { get; set; }
    public double? ActualScore { get; set; }
    public int? ModelVersionId { get; set; }
    public string? ModelVersion { get; set; }
    public string? Algorithm { get; set; }
    public long DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PredictionRequestDto
{
    public int? StudentId { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(10, 25)]
    public int Age { get; set; }
    
    [System.ComponentModel.DataAnnotations.Required]
    public string Gender { get; set; } = string.Empty;
    
    [System.ComponentModel.DataAnnotations.Range(0, 24)]
    public double StudyHours { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public double AttendanceRate { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public double HomeworkCompletionRate { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 20)]
    public double PreviousAverage { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 20)]
    public double PreviousExamScore { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 20)]
    public double MidtermScore { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 30)]
    public int AbsenceDays { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 24)]
    public double SleepHours { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public double ClassParticipation { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 24)]
    public double MobileUsageHours { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public int PracticeTestCount { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 20)]
    public double? ActualScore { get; set; }
}

public class PredictionResultDto
{
    public int Id { get; set; }
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
    public double PredictedScore { get; set; }
    public double Confidence { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public int ModelVersionId { get; set; }
    public string Algorithm { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, double>? FeatureImportance { get; set; }
}
