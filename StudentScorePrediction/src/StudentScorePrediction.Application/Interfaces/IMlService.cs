using StudentScorePrediction.ML.Models;

namespace StudentScorePrediction.Application.Interfaces;

public interface IMlService : IDisposable
{
    Task<string> GenerateDatasetAsync(int recordCount, string filePath);
    Task<TrainingResult> TrainModelAsync(string dataPath, string algorithm, int? maxRecords = null);
    Task<PredictionResult> PredictAsync(StudentInput input);
    Task<ModelEvaluationResult> EvaluateModelAsync(string dataPath, string modelPath);
    Task<ModelComparisonResult> CompareAlgorithmsAsync(string dataPath, int? maxRecords = null);
}
