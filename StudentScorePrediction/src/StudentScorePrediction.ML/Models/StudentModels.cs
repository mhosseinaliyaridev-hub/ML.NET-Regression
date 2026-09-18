namespace StudentScorePrediction.ML.Models;

public class StudentInput
{
    public float Age { get; set; }
    public float Gender { get; set; } // 0 = Female, 1 = Male
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
    public float FinalScore { get; set; } // Label for training
}

public class StudentPrediction
{
    public float Score { get; set; }
}
