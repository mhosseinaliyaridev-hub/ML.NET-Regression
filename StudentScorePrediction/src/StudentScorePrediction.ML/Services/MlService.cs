using Microsoft.Extensions.Logging;
using StudentScorePrediction.ML.DataGeneration;
using StudentScorePrediction.ML.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Transforms;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers.Sdca;
using Microsoft.ML.Data;

namespace StudentScorePrediction.ML.Services;

public class MlService : IMlService
{
    private readonly MLContext _mlContext;
    private readonly string _modelsPath;
    private readonly ILogger<MlService> _logger;
    private ITransformer? _currentModel;
    private ModelMetadata? _currentModelMetadata;

    public MlService(ILogger<MlService> logger)
    {
        _mlContext = new MLContext(seed: 42);
        _modelsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MLModels");
        _logger = logger;

        if (!Directory.Exists(_modelsPath))
        {
            Directory.CreateDirectory(_modelsPath);
        }
    }

    public Task<string> GenerateDatasetAsync(int recordCount, string filePath)
    {
        return Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Starting dataset generation with {Count} records", recordCount);
                var generator = new DatasetGenerator();
                generator.GenerateCsv(filePath, recordCount);
                _logger.LogInformation("Dataset generated successfully at {Path}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dataset");
                throw;
            }
        });
    }

    public async Task<TrainingResult> TrainModelAsync(string dataPath, string algorithm, int? maxRecords = null)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Starting training with algorithm: {Algorithm}", algorithm);
                var startTime = DateTime.Now;

                // Load data
                var dataView = _mlContext.Data.LoadFromTextFile<StudentData>(
                    dataPath, 
                    hasHeader: true, 
                    separatorChar: ',');

                // Apply max records if specified
                if (maxRecords.HasValue && maxRecords.Value > 0)
                {
                    dataView = _mlContext.Data.TakeRows(dataView, maxRecords.Value);
                }

                // Split data: 70% train, 15% validation, 15% test
                var trainTestSplit = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3, seed: 42);
                var trainValidationSplit = _mlContext.Data.TrainTestSplit(trainTestSplit.TrainSet, testFraction: 0.214, seed: 42);

                var trainingData = trainValidationSplit.TrainSet;
                var validationData = trainValidationSplit.TestSet;
                var testData = trainTestSplit.TestSet;

                // Define pipeline
                var pipeline = BuildPipeline(algorithm);

                // Train model
                _logger.LogInformation("Training model...");
                var model = pipeline.Fit(trainingData);
                
                // Evaluate on test data
                var predictions = model.Transform(testData);
                var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                var result = new TrainingResult
                {
                    MAE = metrics.MeanAbsoluteError,
                    MSE = metrics.MeanSquaredError,
                    RMSE = Math.Sqrt(metrics.MeanSquaredError),
                    RSquared = metrics.RSquared,
                    TrainingDuration = duration,
                    Algorithm = algorithm,
                    DatasetSize = maxRecords ?? GetRowCount(dataPath),
                    Success = true
                };

                // Save model
                var modelFileName = $"model_{algorithm}_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                var modelPath = Path.Combine(_modelsPath, modelFileName);
                
                _mlContext.Model.Save(model, trainingData.Schema, modelPath);
                
                // Save metadata
                _currentModelMetadata = new ModelMetadata
                {
                    FileName = modelFileName,
                    Algorithm = algorithm,
                    CreatedDate = DateTime.Now,
                    MAE = metrics.MeanAbsoluteError,
                    MSE = metrics.MeanSquaredError,
                    RMSE = Math.Sqrt(metrics.MeanSquaredError),
                    RSquared = metrics.RSquared,
                    DatasetSize = result.DatasetSize,
                    TrainingDuration = duration
                };

                _currentModel = model;
                _logger.LogInformation("Model trained and saved successfully. R²: {RSquared}", metrics.RSquared);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error training model");
                return new TrainingResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        });
    }

    private IEstimator<ITransformer> BuildPipeline(string algorithm)
    {
        var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Gender", "GenderEncoded")
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("ClassParticipation", "ParticipationEncoded"))
            .Append(_mlContext.Transforms.Concatenate("Features",
                "Age",
                "GenderEncoded",
                "StudyHours",
                "AttendanceRate",
                "HomeworkCompletionRate",
                "PreviousAverage",
                "PreviousExamScore",
                "MidtermScore",
                "AbsenceDays",
                "SleepHours",
                "ParticipationEncoded",
                "MobileUsageHours",
                "PracticeTestCount"))
            .Append(_mlContext.Transforms.NormalizeMinMax("Features"));

        return algorithm.ToLower() switch
        {
            "fasttree" => pipeline.Append(_mlContext.Regression.Trainers.FastTree()),
            "fastforest" => pipeline.Append(_mlContext.Regression.Trainers.FastForest()),
            "sdca" => pipeline.Append(_mlContext.Regression.Trainers.Sdca()),
            _ => pipeline.Append(_mlContext.Regression.Trainers.FastTree())
        };
    }

    public Task<PredictionResult> PredictAsync(StudentInput input)
    {
        return Task.Run(() =>
        {
            try
            {
                if (_currentModel == null || _currentModelMetadata == null)
                {
                    throw new InvalidOperationException("No model loaded. Please train a model first.");
                }

                var startTime = DateTime.Now;
                
                var studentData = new StudentData
                {
                    Age = input.Age,
                    Gender = input.Gender,
                    StudyHours = input.StudyHours,
                    AttendanceRate = input.AttendanceRate,
                    HomeworkCompletionRate = input.HomeworkCompletionRate,
                    PreviousAverage = input.PreviousAverage,
                    PreviousExamScore = input.PreviousExamScore,
                    MidtermScore = input.MidtermScore,
                    AbsenceDays = input.AbsenceDays,
                    SleepHours = input.SleepHours,
                    ClassParticipation = input.ClassParticipation,
                    MobileUsageHours = input.MobileUsageHours,
                    PracticeTestCount = input.PracticeTestCount
                };

                var predictionEngine = _mlContext.Model.CreatePredictionEngine<StudentData, PredictionOutput>(_currentModel);
                var prediction = predictionEngine.Predict(studentData);

                // Clamp prediction to 0-20 range
                var predictedScore = Math.Max(0, Math.Min(20, prediction.Score));

                var duration = DateTime.Now - startTime;

                return new PredictionResult
                {
                    PredictedScore = predictedScore,
                    ModelVersion = _currentModelMetadata.FileName,
                    PredictionTime = DateTime.Now,
                    Duration = duration,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making prediction");
                return Task.FromResult(new PredictionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        });
    }

    public Task<ModelEvaluationResult> EvaluateModelAsync(string dataPath, string modelPath)
    {
        return Task.Run(() =>
        {
            try
            {
                var dataView = _mlContext.Data.LoadFromTextFile<StudentData>(dataPath, hasHeader: true, separatorChar: ',');
                var model = _mlContext.Model.Load(modelPath, out var schema);
                var predictions = model.Transform(dataView);
                var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

                return Task.FromResult(new ModelEvaluationResult
                {
                    MAE = metrics.MeanAbsoluteError,
                    MSE = metrics.MeanSquaredError,
                    RMSE = Math.Sqrt(metrics.MeanSquaredError),
                    RSquared = metrics.RSquared,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating model");
                return Task.FromResult(new ModelEvaluationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        });
    }

    public Task<ModelComparisonResult> CompareAlgorithmsAsync(string dataPath, int? maxRecords = null)
    {
        return Task.Run(async () =>
        {
            var algorithms = new[] { "FastTree", "FastForest", "SDCA" };
            var results = new System.Collections.Generic.List<TrainingResult>();

            foreach (var algo in algorithms)
            {
                var result = await TrainModelAsync(dataPath, algo, maxRecords);
                if (result.Success)
                {
                    results.Add(result);
                }
            }

            return new ModelComparisonResult
            {
                Results = results,
                BestAlgorithm = results.OrderByDescending(r => r.RSquared).FirstOrDefault()?.Algorithm ?? "Unknown",
                Success = results.Any()
            };
        });
    }

    private int GetRowCount(string filePath)
    {
        return System.IO.File.ReadLines(filePath).Count() - 1;
    }

    public void Dispose()
    {
    }
}
