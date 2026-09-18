namespace StudentScorePrediction.Application.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
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
    public double? FinalScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
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

public class UpdateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
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
