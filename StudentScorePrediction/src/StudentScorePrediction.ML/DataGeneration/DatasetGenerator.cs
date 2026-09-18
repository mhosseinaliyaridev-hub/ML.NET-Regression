using StudentScorePrediction.ML.Models;

namespace StudentScorePrediction.ML.DataGeneration;

public class DatasetGenerator
{
    private static readonly Random _random = new();

    public static IEnumerable<StudentInput> Generate(int recordCount, int seed = 42)
    {
        _random = new Random(seed);

        var firstNames = new[] { "Ali", "Reza", "Mohammad", "Hossein", "Ahmad", "Maryam", "Zahra", "Fatima", "Sara", "Narges" };
        var lastNames = new[] { "Hosseini", "Karimi", "Moradi", "Rashidi", "Azizi", "Mohammadi", "Rezaei", "Ahmadi", "Hasani", "Jafari" };

        for (int i = 0; i < recordCount; i++)
        {
            var student = GenerateRealisticStudent();

            // Add some missing values (about 1% of data)
            if (_random.NextDouble() < 0.01)
            {
                student.SleepHours = float.NaN;
            }
            if (_random.NextDouble() < 0.01)
            {
                student.ClassParticipation = float.NaN;
            }

            // Add some outliers (about 0.5% of data)
            if (_random.NextDouble() < 0.005)
            {
                student.StudyHours = _random.Next(15, 20); // Unusually high
            }
            if (_random.NextDouble() < 0.005)
            {
                student.AttendanceRate = _random.Next(0, 30); // Unusually low
            }

            yield return student;
        }
    }

    private static StudentInput GenerateRealisticStudent()
    {
        // Base values with realistic distributions
        var age = _random.Next(10, 26);
        var gender = _random.Next(0, 2); // 0 or 1
        var studyHours = Math.Max(0, Math.Min(12, _random.NextDouble() * 8 + 1)); // 1-9 hours typically
        var attendanceRate = Math.Max(0, Math.Min(100, _random.NextDouble() * 40 + 60)); // 60-100% typically
        var homeworkCompletionRate = Math.Max(0, Math.Min(100, _random.NextDouble() * 50 + 50)); // 50-100%
        var previousAverage = Math.Max(0, Math.Min(20, _random.NextDouble() * 8 + 10)); // 10-18 typically
        var previousExamScore = Math.Max(0, Math.Min(20, _random.NextDouble() * 10 + 8)); // 8-18
        var midtermScore = Math.Max(0, Math.Min(20, _random.NextDouble() * 12 + 6)); // 6-18
        var absenceDays = Math.Max(0, Math.Min(30, _random.NextDouble() * 10)); // 0-10 days typically
        var sleepHours = Math.Max(4, Math.Min(12, _random.NextDouble() * 4 + 6)); // 6-10 hours
        var classParticipation = Math.Max(0, Math.Min(100, _random.NextDouble() * 60 + 40)); // 40-100%
        var mobileUsageHours = Math.Max(0, Math.Min(10, _random.NextDouble() * 6)); // 0-6 hours
        var practiceTestCount = Math.Max(0, Math.Min(50, _random.NextDouble() * 30)); // 0-30 tests

        // Calculate FinalScore based on realistic relationships
        // This creates a meaningful correlation between features and target
        var baseScore = 10.0f;

        // Positive influences
        baseScore += (float)(studyHours * 0.6); // More study → higher score
        baseScore += (float)(attendanceRate * 0.08); // Better attendance → higher score
        baseScore += (float)(homeworkCompletionRate * 0.05); // More homework → higher score
        baseScore += (float)(previousAverage * 0.25); // Previous performance matters
        baseScore += (float)(midtermScore * 0.3); // Midterm is strong predictor
        baseScore += (float)(classParticipation * 0.04); // Participation helps
        baseScore += (float)(practiceTestCount * 0.03); // Practice improves score
        baseScore += (float)(sleepHours * 0.3); // Good sleep helps

        // Negative influences
        baseScore -= (float)(absenceDays * 0.4); // More absences → lower score
        baseScore -= (float)(mobileUsageHours * 0.5); // More mobile usage → lower score

        // Add noise to make it realistic (not perfectly predictable)
        var noise = (float)(_random.NextGaussian() * 1.5);
        var finalScore = baseScore + noise;

        // Clamp to valid range [0, 20]
        finalScore = Math.Max(0, Math.Min(20, finalScore));

        return new StudentInput
        {
            Age = age,
            Gender = gender,
            StudyHours = (float)Math.Round(studyHours, 2),
            AttendanceRate = (float)Math.Round(attendanceRate, 2),
            HomeworkCompletionRate = (float)Math.Round(homeworkCompletionRate, 2),
            PreviousAverage = (float)Math.Round(previousAverage, 2),
            PreviousExamScore = (float)Math.Round(previousExamScore, 2),
            MidtermScore = (float)Math.Round(midtermScore, 2),
            AbsenceDays = (float)Math.Round(absenceDays, 2),
            SleepHours = (float)Math.Round(sleepHours, 2),
            ClassParticipation = (float)Math.Round(classParticipation, 2),
            MobileUsageHours = (float)Math.Round(mobileUsageHours, 2),
            PracticeTestCount = (float)Math.Round(practiceTestCount, 2),
            FinalScore = (float)Math.Round(finalScore, 2)
        };
    }

    // Box-Muller transform for Gaussian random numbers
    private static double NextGaussian(this Random random)
    {
        var u1 = 1.0 - random.NextDouble();
        var u2 = 1.0 - random.NextDouble();
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return randStdNormal;
    }

    public static void SaveToCsv(IEnumerable<StudentInput> students, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Age,Gender,StudyHours,AttendanceRate,HomeworkCompletionRate,PreviousAverage,PreviousExamScore,MidtermScore,AbsenceDays,SleepHours,ClassParticipation,MobileUsageHours,PracticeTestCount,FinalScore");

        foreach (var student in students)
        {
            writer.WriteLine($"{student.Age},{student.Gender},{student.StudyHours:F2},{student.AttendanceRate:F2},{student.HomeworkCompletionRate:F2},{student.PreviousAverage:F2},{student.PreviousExamScore:F2},{student.MidtermScore:F2},{student.AbsenceDays:F2},{student.SleepHours:F2},{student.ClassParticipation:F2},{student.MobileUsageHours:F2},{student.PracticeTestCount:F2},{student.FinalScore:F2}");
        }
    }

    public static IEnumerable<StudentInput> LoadFromCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        reader.ReadLine(); // Skip header

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 14) continue;

            yield return new StudentInput
            {
                Age = float.Parse(parts[0]),
                Gender = float.Parse(parts[1]),
                StudyHours = ParseFloatOrNaN(parts[2]),
                AttendanceRate = ParseFloatOrNaN(parts[3]),
                HomeworkCompletionRate = ParseFloatOrNaN(parts[4]),
                PreviousAverage = ParseFloatOrNaN(parts[5]),
                PreviousExamScore = ParseFloatOrNaN(parts[6]),
                MidtermScore = ParseFloatOrNaN(parts[7]),
                AbsenceDays = ParseFloatOrNaN(parts[8]),
                SleepHours = ParseFloatOrNaN(parts[9]),
                ClassParticipation = ParseFloatOrNaN(parts[10]),
                MobileUsageHours = ParseFloatOrNaN(parts[11]),
                PracticeTestCount = ParseFloatOrNaN(parts[12]),
                FinalScore = ParseFloatOrNaN(parts[13])
            };
        }
    }

    private static float ParseFloatOrNaN(string value)
    {
        return float.TryParse(value, out var result) ? result : float.NaN;
    }
}
