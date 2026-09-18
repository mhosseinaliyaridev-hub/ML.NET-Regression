using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers.Sdca;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.ML.DataGeneration;
using StudentScorePrediction.ML.Models;

namespace StudentScorePrediction.ML.Services;

public class MlService : IMlService
{
    private readonly MLContext _mlContext;
    private readonly string _modelPath;
    private ITransformer? _trainedModel;
    private PredictionEngine<StudentInput, StudentPrediction>? _predictionEngine;

    public MlService()
    {
        _mlContext = new MLContext(seed: 42);
        _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ML", "Models", "StudentScoreModel.zip");
    }

    public async Task<ModelMetricsDto> TrainAsync(string algorithm, IEnumerable<StudentInput> data, Action<string>? onStatusChange = null, CancellationToken cancellationToken = default)
    {
        await Task.Yield(); // Ensure we're not blocking

        var dataList = data.ToList();
        var dataView = _mlContext.Data.LoadFromEnumerable(dataList);

        // Split data: 70% train, 15% validation, 15% test
        var trainTestSplit = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.15, seed: 42);
        var trainValidationSplit = _mlContext.Data.TrainTestSplit(trainTestSplit.TrainSet, testFraction: 0.176, seed: 42); // 0.176 of 0.85 ≈ 0.15

        var trainingData = trainValidationSplit.TrainSet;
        var validationData = trainValidationSplit.TestSet;
        var testData = trainTestSplit.TestSet;

        onStatusChange?.Invoke("Preparing Data");

        // Define pipeline
        var pipeline = CreatePipeline(algorithm);

        onStatusChange?.Invoke("Training");

        // Train model
        var startTime = DateTime.UtcNow;
        var model = pipeline.Fit(trainingData);
        var trainingDuration = DateTime.UtcNow - startTime;

        onStatusChange?.Invoke("Evaluating");

        // Evaluate on test data
        var predictions = model.Transform(testData);
        var metrics = _mlContext.Regression.Evaluate(predictions);

        var result = new ModelMetricsDto
        {
            MAE = metrics.MeanAbsoluteError,
            MSE = metrics.MeanSquaredError,
            RMSE = Math.Sqrt(metrics.MeanSquaredError),
            RSquared = metrics.RSquared,
            TrainingDuration = trainingDuration,
            DatasetSize = dataList.Count,
            Algorithm = algorithm
        };

        // Try to get feature importance if available
        if (model.LastTransformer is FeatureImportanceCalculator featureImportance)
        {
            try
            {
                var weights = model.LastTransformer.GetFeatureWeights();
                result.FeatureImportance = new Dictionary<string, double>();
                // Map weights to feature names (simplified)
                var featureNames = new[] { "Age", "Gender", "StudyHours", "AttendanceRate", "HomeworkCompletionRate", 
                    "PreviousAverage", "PreviousExamScore", "MidtermScore", "AbsenceDays", "SleepHours", 
                    "ClassParticipation", "MobileUsageHours", "PracticeTestCount" };
                
                for (int i = 0; i < Math.Min(featureNames.Length, weights.Count); i++)
                {
                    result.FeatureImportance[featureNames[i]] = Math.Abs(weights[i]);
                }
            }
            catch
            {
                // Feature importance not available for this model
            }
        }

        return result;
    }

    private IEstimator<ITransformer> CreatePipeline(string algorithm)
    {
        var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "FinalScore")
            .Append(_mlContext.Transforms.ReplaceMissingValues())
            .Append(_mlContext.Transforms.NormalizeMinMax())
            .Append(_mlContext.Transforms.Concatenate("Features", nameof(StudentInput.Age), nameof(StudentInput.Gender),
                nameof(StudentInput.StudyHours), nameof(StudentInput.AttendanceRate), nameof(StudentInput.HomeworkCompletionRate),
                nameof(StudentInput.PreviousAverage), nameof(StudentInput.PreviousExamScore), nameof(StudentInput.MidtermScore),
                nameof(StudentInput.AbsenceDays), nameof(StudentInput.SleepHours), nameof(StudentInput.ClassParticipation),
                nameof(StudentInput.MobileUsageHours), nameof(StudentInput.PracticeTestCount)));

        return algorithm switch
        {
            "SdcaRegression" => pipeline.Append(_mlContext.Regression.Trainers.Sdca()),
            "FastForestRegression" => pipeline.Append(_mlContext.Regression.Trainers.FastForest()),
            _ => pipeline.Append(_mlContext.Regression.Trainers.FastTree()) // Default to FastTree
        };
    }

    public void SaveModel(ITransformer model, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        _mlContext.Model.Save(model, _mlContext.DataView.Schema, path);
        _trainedModel = model;
    }

    public ITransformer LoadModel(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Model file not found: {path}");
        }

        _trainedModel = _mlContext.Model.Load(path, out var schema);
        return _trainedModel;
    }

    public float Predict(StudentInput input)
    {
        if (_trainedModel == null)
        {
            throw new InvalidOperationException("Model not loaded. Call LoadModel first.");
        }

        if (_predictionEngine == null)
        {
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<StudentInput, StudentPrediction>(_trainedModel);
        }

        var prediction = _predictionEngine.Predict(input);
        return Math.Max(0, Math.Min(20, prediction.Score)); // Clamp to [0, 20]
    }

    public async Task<IEnumerable<StudentInput>> GenerateDatasetAsync(int recordCount, string filePath, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        var students = DatasetGenerator.Generate(recordCount).ToList();
        DatasetGenerator.SaveToCsv(students, filePath);
        return students;
    }

    public IEnumerable<StudentInput> LoadDataset(string filePath)
    {
        return DatasetGenerator.LoadFromCsv(filePath);
    }

    public DatasetStatisticsDto GetDatasetStatistics(string filePath)
    {
        var students = DatasetGenerator.LoadFromCsv(filePath).ToList();
        
        var stats = new DatasetStatisticsDto
        {
            RecordCount = students.Count,
            MissingValues = students.Count(s => float.IsNaN(s.SleepHours) || float.IsNaN(s.ClassParticipation)),
            Outliers = students.Count(s => s.StudyHours > 14 || s.AttendanceRate < 30),
            FeatureStats = new Dictionary<string, FeatureStats>()
        };

        // Calculate statistics for each feature
        if (students.Any())
        {
            stats.FeatureStats["StudyHours"] = new FeatureStats
            {
                Min = students.Min(s => s.StudyHours),
                Max = students.Max(s => s.StudyHours),
                Average = students.Average(s => s.StudyHours)
            };

            stats.FeatureStats["AttendanceRate"] = new FeatureStats
            {
                Min = students.Min(s => s.AttendanceRate),
                Max = students.Max(s => s.AttendanceRate),
                Average = students.Average(s => s.AttendanceRate)
            };

            stats.FeatureStats["FinalScore"] = new FeatureStats
            {
                Min = students.Min(s => s.FinalScore),
                Max = students.Max(s => s.FinalScore),
                Average = students.Average(s => s.FinalScore)
            };
        }

        return stats;
    }
}

public interface IMlService
{
    Task<ModelMetricsDto> TrainAsync(string algorithm, IEnumerable<StudentInput> data, Action<string>? onStatusChange = null, CancellationToken cancellationToken = default);
    void SaveModel(ITransformer model, string path);
    ITransformer LoadModel(string path);
    float Predict(StudentInput input);
    Task<IEnumerable<StudentInput>> GenerateDatasetAsync(int recordCount, string filePath, CancellationToken cancellationToken = default);
    IEnumerable<StudentInput> LoadDataset(string filePath);
    DatasetStatisticsDto GetDatasetStatistics(string filePath);
}

public class ModelMetricsDto
{
    public double MAE { get; set; }
    public double MSE { get; set; }
    public double RMSE { get; set; }
    public double RSquared { get; set; }
    public TimeSpan TrainingDuration { get; set; }
    public int DatasetSize { get; set; }
    public string Algorithm { get; set; } = string.Empty;
    public Dictionary<string, double>? FeatureImportance { get; set; }
}
