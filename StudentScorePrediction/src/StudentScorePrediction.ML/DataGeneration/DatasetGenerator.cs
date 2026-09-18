using StudentScorePrediction.Domain.Enums;

namespace StudentScorePrediction.ML.DataGeneration;

public class DatasetGenerator
{
    private readonly Random _random = new();
    private static readonly string[] FirstNames = { "Ali", "Reza", "Mohammad", "Hossein", "Ahmad", "Maryam", "Zahra", "Fateme", "Narges", "Leila" };
    private static readonly string[] LastNames = { "Hosseini", "Karimi", "Moradi", "Rashidi", "Azizi", "Mohammadi", "Rezaei", "Ahmadi", "Hasani", "Jafari" };

    public List<StudentData> GenerateStudents(int count)
    {
        var students = new List<StudentData>(count);

        for (int i = 0; i < count; i++)
        {
            var student = GenerateStudent(i);
            
            // Add some missing values (about 1%)
            if (_random.NextDouble() < 0.01)
                student.SleepHours = null;
            
            if (_random.NextDouble() < 0.01)
                student.ClassParticipation = null;

            // Add some outliers (about 0.5%)
            if (_random.NextDouble() < 0.005)
                student.StudyHours = _random.NextFloat(20, 30); // Unrealistic study hours
            
            if (_random.NextDouble() < 0.005)
                student.AttendanceRate = _random.NextFloat(-10, 0); // Negative attendance (error)

            students.Add(student);
        }

        return students;
    }

    private StudentData GenerateStudent(int id)
    {
        var gender = _random.NextEnum<Gender>();
        
        // Generate correlated features
        var baseAbility = _random.NextFloat(0, 1); // Hidden variable representing student ability
        
        var studyHours = Math.Clamp(_random.NextGaussian(4 + baseAbility * 6, 2), 0, 16);
        var attendanceRate = Math.Clamp(_random.NextGaussian(60 + baseAbility * 35, 15), 0, 100);
        var homeworkCompletionRate = Math.Clamp(_random.NextGaussian(50 + baseAbility * 45, 12), 0, 100);
        var previousAverage = Math.Clamp(_random.NextGaussian(8 + baseAbility * 10, 3), 0, 20);
        var previousExamScore = Math.Clamp(_random.NextGaussian(previousAverage, 2), 0, 20);
        var midtermScore = Math.Clamp(_random.NextGaussian(previousAverage + _random.NextFloat(-1, 1), 2), 0, 20);
        var absenceDays = Math.Max(0, (int)Math.Round((100 - attendanceRate) / 10 + _random.NextGaussian(0, 2)));
        var sleepHours = Math.Clamp(_random.NextGaussian(7, 1.5), 3, 12);
        var classParticipation = Math.Clamp(_random.NextGaussian(40 + baseAbility * 50, 15), 0, 100);
        var mobileUsageHours = Math.Clamp(_random.NextGaussian(4 - baseAbility * 2, 1.5), 0, 12);
        var practiceTestCount = (int)Math.Max(0, Math.Round(_random.NextGaussian(3 + baseAbility * 7, 2)));

        // Calculate final score based on features with realistic relationships
        float finalScore = CalculateFinalScore(
            studyHours, attendanceRate, homeworkCompletionRate, previousAverage,
            midtermScore, absenceDays, sleepHours, classParticipation,
            mobileUsageHours, practiceTestCount, baseAbility);

        return new StudentData
        {
            Id = id,
            FirstName = FirstNames[_random.Next(FirstNames.Length)],
            LastName = LastNames[_random.Next(LastNames.Length)],
            Age = _random.Next(10, 26),
            Gender = gender,
            StudyHours = studyHours,
            AttendanceRate = attendanceRate,
            HomeworkCompletionRate = homeworkCompletionRate,
            PreviousAverage = previousAverage,
            PreviousExamScore = previousExamScore,
            MidtermScore = midtermScore,
            AbsenceDays = absenceDays,
            SleepHours = sleepHours,
            ClassParticipation = classParticipation,
            MobileUsageHours = mobileUsageHours,
            PracticeTestCount = practiceTestCount,
            FinalScore = finalScore
        };
    }

    private float CalculateFinalScore(
        float studyHours, float attendanceRate, float homeworkCompletionRate,
        float previousAverage, float midtermScore, int absenceDays,
        float sleepHours, float classParticipation, float mobileUsageHours,
        int practiceTestCount, float baseAbility)
    {
        // Weighted combination of features
        float score = 0;

        // Positive factors
        score += studyHours * 0.4f; // More study → higher score
        score += attendanceRate * 0.15f; // Better attendance → higher score
        score += homeworkCompletionRate * 0.1f; // More homework → higher score
        score += previousAverage * 0.3f; // Better previous performance → higher score
        score += midtermScore * 0.25f; // Better midterm → higher score
        score += classParticipation * 0.08f; // More participation → higher score
        score += practiceTestCount * 0.15f; // More practice → higher score

        // Sleep effect (optimal around 7-8 hours)
        if (sleepHours >= 6 && sleepHours <= 9)
            score += sleepHours * 0.1f;
        else if (sleepHours < 6)
            score -= (6 - sleepHours) * 0.3f; // Penalty for too little sleep
        else
            score -= (sleepHours - 9) * 0.1f; // Slight penalty for too much sleep

        // Negative factors
        score -= absenceDays * 0.2f; // More absences → lower score
        score -= mobileUsageHours * 0.15f; // More mobile usage → lower score

        // Base ability factor
        score += baseAbility * 3;

        // Add some noise
        score += _random.NextGaussian(0, 1);

        // Clamp to 0-20 range
        return Math.Clamp(score, 0, 20);
    }
}

public class StudentData
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
    public float? SleepHours { get; set; }
    public float? ClassParticipation { get; set; }
    public float MobileUsageHours { get; set; }
    public int PracticeTestCount { get; set; }
    public float FinalScore { get; set; }
}
