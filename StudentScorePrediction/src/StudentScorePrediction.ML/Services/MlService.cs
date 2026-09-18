using Microsoft.ML;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Enums;
using StudentScorePrediction.ML.DataGeneration;
using System.Globalization;

namespace StudentScorePrediction.ML.Services;

public class MlService : IMlService
{
    private readonly string _modelsPath;
    private readonly string _datasetPath;
    private readonly ILogger<MlService> _logger;
    private ITransformer? _currentModel;
    private string? _currentModelVersion;

    public MlService(ILogger<MlService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var basePath = AppContext.BaseDirectory;
        _modelsPath = Path.Combine(basePath, "MLModels");
        _datasetPath = Path.Combine(basePath, "Datasets");
        
        Directory.CreateDirectory(_modelsPath);
        Directory.CreateDirectory(_datasetPath);
    }

    public async Task<string> GenerateDatasetAsync(int recordCount, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting dataset generation with {RecordCount} records", recordCount);
            
            var generator = new DatasetGenerator();
            var students = generator.GenerateStudents(recordCount);
            
            var fileName = $"student_dataset_{recordCount}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(_datasetPath, fileName);
            
            await SaveToCsvAsync(students, filePath, cancellationToken);
            
            _logger.LogInformation("Dataset generated successfully: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating dataset");
            throw;
        }
    }

    public async Task<TrainingResult> TrainModelAsync(
        string datasetPath,
        AlgorithmType algorithm,
        string modelVersion,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting model training with algorithm {Algorithm}", algorithm);
            
            if (!File.Exists(datasetPath))
                throw new FileNotFoundException("Dataset file not found", datasetPath);

            var mlContext = new MLContext(seed: 0);
            var dataView = await LoadFromCsvAsync(mlContext, datasetPath);

            // Split data: 70% train, 15% validation, 15% test
            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3, seed: 0);
            var trainValidationData = mlContext.Data.TrainTestSplit(trainTestData.TestSet, testFraction: 0.5, seed: 0);
            
            var trainingData = trainTestData.TrainSet;
            var testData = trainValidationData.TestSet;

            // Build pipeline
            var pipeline = BuildPipeline(mlContext, algorithm);

            _logger.LogInformation("Training model...");
            var startTime = DateTime.UtcNow;
            var model = pipeline.Fit(trainingData);
            var endTime = DateTime.UtcNow;
            _logger.LogInformation("Model training completed");

            // Evaluate
            var metrics = EvaluateModel(mlContext, model, testData);
            
            // Save model
            var modelPath = SaveModel(model, modelVersion);

            _logger.LogInformation("Model trained successfully. RMSE: {Rmse}, R²: {R2}", 
                metrics.RootMeanSquaredError, metrics.RSquared);

            return new TrainingResult
            {
                ModelPath = modelPath,
                ModelVersion = modelVersion,
                Algorithm = algorithm,
                Metrics = new RegressionMetrics
                {
                    MeanAbsoluteError = metrics.MeanAbsoluteError,
                    MeanSquaredError = metrics.MeanSquaredError,
                    RootMeanSquaredError = metrics.RootMeanSquaredError,
                    RSquared = metrics.RSquared
                },
                TrainingDuration = endTime - startTime,
                DatasetSize = GetRowCount(datasetPath)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training model");
            throw;
        }
    }

    public PredictionResult Predict(StudentInput input)
    {
        try
        {
            if (_currentModel == null)
                throw new InvalidOperationException("No model loaded. Please train a model first.");

            var mlContext = new MLContext(seed: 0);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<StudentInput, PredictionOutput>(_currentModel);
            
            var startTime = DateTime.UtcNow;
            var output = predictionEngine.Predict(input);
            var duration = DateTime.UtcNow - startTime;

            // Clamp prediction to 0-20 range
            var clampedScore = Math.Max(0, Math.Min(20, output.FinalScore));

            return new PredictionResult
            {
                PredictedScore = clampedScore,
                ModelVersion = _currentModelVersion ?? "Unknown",
                PredictionTime = startTime,
                Duration = duration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making prediction");
            throw;
        }
    }

    public async Task<PredictionResult> PredictWithModelAsync(string modelPath, StudentInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            var mlContext = new MLContext(seed: 0);
            var model = mlContext.Model.Load(modelPath, out var schema);
            
            var predictionEngine = mlContext.Model.CreatePredictionEngine<StudentInput, PredictionOutput>(model);
            
            var startTime = DateTime.UtcNow;
            var output = predictionEngine.Predict(input);
            var duration = DateTime.UtcNow - startTime;

            // Clamp prediction to 0-20 range
            var clampedScore = Math.Max(0, Math.Min(20, output.FinalScore));

            return new PredictionResult
            {
                PredictedScore = clampedScore,
                ModelVersion = Path.GetFileNameWithoutExtension(modelPath),
                PredictionTime = startTime,
                Duration = duration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making prediction with model {ModelPath}", modelPath);
            throw;
        }
    }

    public async Task<IEnumerable<ModelEvaluation>> CompareAlgorithmsAsync(string datasetPath, CancellationToken cancellationToken = default)
    {
        var results = new List<ModelEvaluation>();
        var algorithms = Enum.GetValues<AlgorithmType>();

        foreach (var algorithm in algorithms)
        {
            try
            {
                var result = await TrainModelAsync(datasetPath, algorithm, $"temp_{algorithm}", cancellationToken);
                
                results.Add(new ModelEvaluation
                {
                    Algorithm = algorithm,
                    MAE = result.Metrics.MeanAbsoluteError,
                    MSE = result.Metrics.MeanSquaredError,
                    RMSE = result.Metrics.RootMeanSquaredError,
                    RSquared = result.Metrics.RSquared,
                    TrainingDuration = result.TrainingDuration
                });
                
                // Clean up temp model
                var tempPath = Path.Combine(_modelsPath, $"temp_{algorithm}.zip");
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to train model with algorithm {Algorithm}", algorithm);
                results.Add(new ModelEvaluation
                {
                    Algorithm = algorithm,
                    Error = ex.Message
                });
            }
        }

        return results;
    }

    public async Task LoadModelAsync(string modelPath, string version, CancellationToken cancellationToken = default)
    {
        var mlContext = new MLContext(seed: 0);
        _currentModel = mlContext.Model.Load(modelPath, out var schema);
        _currentModelVersion = version;
        
        _logger.LogInformation("Model loaded: {Version}", version);
    }

    public async Task<DatasetStatisticsDto> GetDatasetStatisticsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var students = await LoadFromCsvAsync(filePath);
        
        var validStudents = students.Where(s => s.FinalScore >= 0 && s.FinalScore <= 20).ToList();
        
        var stats = new DatasetStatisticsDto
        {
            TotalRecords = validStudents.Count,
            FeatureCount = 13,
            MissingValues = students.Count(s => s.SleepHours == null || s.ClassParticipation == null),
            MinValues = new Dictionary<string, float>
            {
                ["StudyHours"] = validStudents.Min(s => s.StudyHours),
                ["AttendanceRate"] = validStudents.Min(s => s.AttendanceRate),
                ["FinalScore"] = validStudents.Min(s => s.FinalScore)
            },
            MaxValues = new Dictionary<string, float>
            {
                ["StudyHours"] = validStudents.Max(s => s.StudyHours),
                ["AttendanceRate"] = validStudents.Max(s => s.AttendanceRate),
                ["FinalScore"] = validStudents.Max(s => s.FinalScore)
            },
            AverageValues = new Dictionary<string, float>
            {
                ["StudyHours"] = validStudents.Average(s => s.StudyHours),
                ["AttendanceRate"] = validStudents.Average(s => s.AttendanceRate),
                ["FinalScore"] = validStudents.Average(s => s.FinalScore)
            }
        };

        return stats;
    }

    private IEstimator<ITransformer> BuildPipeline(MLContext mlContext, AlgorithmType algorithm)
    {
        var pipeline = mlContext.Transforms.Conversion.MapValueToKey(nameof(StudentInput.Gender))
            .Append(mlContext.Transforms.NormalizeMinMax(
                nameof(StudentInput.Age),
                nameof(StudentInput.StudyHours),
                nameof(StudentInput.AttendanceRate),
                nameof(StudentInput.HomeworkCompletionRate),
                nameof(StudentInput.PreviousAverage),
                nameof(StudentInput.PreviousExamScore),
                nameof(StudentInput.MidtermScore),
                nameof(StudentInput.AbsenceDays),
                nameof(StudentInput.SleepHours),
                nameof(StudentInput.ClassParticipation),
                nameof(StudentInput.MobileUsageHours),
                nameof(StudentInput.PracticeTestCount)))
            .Append(mlContext.Transforms.Concatenate("Features",
                nameof(StudentInput.Age),
                nameof(StudentInput.StudyHours),
                nameof(StudentInput.AttendanceRate),
                nameof(StudentInput.HomeworkCompletionRate),
                nameof(StudentInput.PreviousAverage),
                nameof(StudentInput.PreviousExamScore),
                nameof(StudentInput.MidtermScore),
                nameof(StudentInput.AbsenceDays),
                nameof(StudentInput.SleepHours),
                nameof(StudentInput.ClassParticipation),
                nameof(StudentInput.MobileUsageHours),
                nameof(StudentInput.PracticeTestCount),
                nameof(StudentInput.Gender)));

        switch (algorithm)
        {
            case AlgorithmType.SdcaRegression:
                pipeline = pipeline.Append(mlContext.Regression.Trainers.Sdca());
                break;
            case AlgorithmType.FastTreeRegression:
                pipeline = pipeline.Append(mlContext.Regression.Trainers.FastTree());
                break;
            case AlgorithmType.FastForestRegression:
                pipeline = pipeline.Append(mlContext.Regression.Trainers.FastForest());
                break;
            default:
                pipeline = pipeline.Append(mlContext.Regression.Trainers.FastTree());
                break;
        }

        return pipeline.Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
    }

    private RegressionMetrics EvaluateModel(MLContext mlContext, ITransformer model, IDataView testData)
    {
        var predictions = model.Transform(testData);
        return mlContext.Regression.Evaluate(predictions);
    }

    private string SaveModel(ITransformer model, string version)
    {
        var fileName = $"{version}.zip";
        var filePath = Path.Combine(_modelsPath, fileName);
        
        using var fs = File.Create(filePath);
        var mlContext = new MLContext(seed: 0);
        mlContext.Model.Save(model, null, fs);
        
        _logger.LogInformation("Model saved to {FilePath}", filePath);
        return filePath;
    }

    private int GetRowCount(string filePath)
    {
        var lines = File.ReadLines(filePath);
        return lines.Count() - 1; // Exclude header
    }

    private async Task<List<StudentData>> LoadFromCsvAsync(string filePath)
    {
        var students = new List<StudentData>();
        
        await using var reader = new StreamReader(filePath);
        var header = await reader.ReadLineAsync(); // Skip header
        
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 14) continue;

            students.Add(new StudentData
            {
                Id = int.Parse(parts[0]),
                FirstName = parts[1],
                LastName = parts[2],
                Age = int.Parse(parts[3]),
                Gender = parts[4] == "Male" ? Gender.Male : Gender.Female,
                StudyHours = float.Parse(parts[5], CultureInfo.InvariantCulture),
                AttendanceRate = float.Parse(parts[6], CultureInfo.InvariantCulture),
                HomeworkCompletionRate = float.Parse(parts[7], CultureInfo.InvariantCulture),
                PreviousAverage = float.Parse(parts[8], CultureInfo.InvariantCulture),
                PreviousExamScore = float.Parse(parts[9], CultureInfo.InvariantCulture),
                MidtermScore = float.Parse(parts[10], CultureInfo.InvariantCulture),
                AbsenceDays = int.Parse(parts[11]),
                SleepHours = string.IsNullOrEmpty(parts[12]) ? null : float.Parse(parts[12], CultureInfo.InvariantCulture),
                ClassParticipation = string.IsNullOrEmpty(parts[13]) ? null : float.Parse(parts[13], CultureInfo.InvariantCulture),
                MobileUsageHours = float.Parse(parts[14], CultureInfo.InvariantCulture),
                PracticeTestCount = int.Parse(parts[15]),
                FinalScore = float.Parse(parts[16], CultureInfo.InvariantCulture)
            });
        }

        return students;
    }

    private async Task<IDataView> LoadFromCsvAsync(MLContext mlContext, string filePath)
    {
        return mlContext.Data.LoadFromTextFile<StudentInput>(
            path: filePath,
            hasHeader: true,
            separatorChar: ',');
    }

    private async Task SaveToCsvAsync(List<StudentData> students, string filePath, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(filePath);
        
        // Write header
        await writer.WriteLineAsync("Id,FirstName,LastName,Age,Gender,StudyHours,AttendanceRate,HomeworkCompletionRate,PreviousAverage,PreviousExamScore,MidtermScore,AbsenceDays,SleepHours,ClassParticipation,MobileUsageHours,PracticeTestCount,FinalScore");
        
        foreach (var student in students)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var sleepHours = student.SleepHours.HasValue ? student.SleepHours.Value.ToString(CultureInfo.InvariantCulture) : "";
            var classParticipation = student.ClassParticipation.HasValue ? student.ClassParticipation.Value.ToString(CultureInfo.InvariantCulture) : "";

            await writer.WriteLineAsync($"{student.Id},{student.FirstName},{student.LastName},{student.Age},{student.Gender},{student.StudyHours.ToString(CultureInfo.InvariantCulture)},{student.AttendanceRate.ToString(CultureInfo.InvariantCulture)},{student.HomeworkCompletionRate.ToString(CultureInfo.InvariantCulture)},{student.PreviousAverage.ToString(CultureInfo.InvariantCulture)},{student.PreviousExamScore.ToString(CultureInfo.InvariantCulture)},{student.MidtermScore.ToString(CultureInfo.InvariantCulture)},{student.AbsenceDays},{sleepHours},{classParticipation},{student.MobileUsageHours.ToString(CultureInfo.InvariantCulture)},{student.PracticeTestCount},{student.FinalScore.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}
