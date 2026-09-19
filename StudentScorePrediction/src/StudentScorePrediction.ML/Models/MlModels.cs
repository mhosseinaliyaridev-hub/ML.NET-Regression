namespace StudentScorePrediction.ML.Models;

public class InputData
{
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
    public string ClassParticipation { get; set; } = string.Empty;
    public float MobileUsageHours { get; set; }
    public float PracticeTestCount { get; set; }
    
    // Label - the value we want to predict
    public float FinalScore { get; set; }
}

public class PredictionOutput
{
    public float Score { get; set; }
}

public class TrainingResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
    public TimeSpan TrainingDuration { get; set; }
    public int DatasetSize { get; set; }
}

public class PredictionResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public double PredictedScore { get; set; }
    public string ModelVersion { get; set; } = string.Empty;
    public DateTime PredictionTime { get; set; }
    public TimeSpan Duration { get; set; }
}

public class ModelEvaluationResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
}

public class ModelComparisonResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string BestAlgorithm { get; set; } = string.Empty;
    public List<TrainingResult> Results { get; set; } = new();
}

public class ModelMetadata
{
    public string FileName { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
    public int DatasetSize { get; set; }
    public TimeSpan TrainingDuration { get; set; }
}

public class StudentInput
{
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
    public string ClassParticipation { get; set; } = string.Empty;
    public float MobileUsageHours { get; set; }
    public float PracticeTestCount { get; set; }
}
