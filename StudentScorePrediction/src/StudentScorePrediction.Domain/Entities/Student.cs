using StudentScorePrediction.Domain.Enums;

namespace StudentScorePrediction.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public float StudyHours { get; set; }
    public float AttendanceRate { get; set; }
    public float HomeworkCompletionRate { get; set; }
    public float PreviousAverage { get; set; }
    public float PreviousExamScore { get; set; }
    public float MidtermScore { get; set; }
    public int AbsenceDays { get; set; }
    public float SleepHours { get; set; }
    public float ClassParticipation { get; set; }
    public float MobileUsageHours { get; set; }
    public int PracticeTestCount { get; set; }
    public float? FinalScore { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
}
