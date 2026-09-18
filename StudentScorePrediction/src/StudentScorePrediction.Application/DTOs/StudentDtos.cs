namespace StudentScorePrediction.Application.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateStudentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    
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
}

public class UpdateStudentDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(10, 25)]
    public int? Age { get; set; }
    
    public string? Gender { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 24)]
    public double? StudyHours { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public double? AttendanceRate { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public double? HomeworkCompletionRate { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 20)]
    public double? PreviousAverage { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 20)]
    public double? PreviousExamScore { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 20)]
    public double? MidtermScore { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 30)]
    public int? AbsenceDays { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 24)]
    public double? SleepHours { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public double? ClassParticipation { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 24)]
    public double? MobileUsageHours { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(0, 100)]
    public int? PracticeTestCount { get; set; }
}

public class StudentInput
{
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
