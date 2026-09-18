using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Domain.Enums;
using StudentScorePrediction.Infrastructure.Repositories;
using StudentScorePrediction.ML.Services;

namespace StudentScorePrediction.Application.Services;

public class TrainingService : ITrainingService
{
    private readonly IMlService _mlService;
    private readonly ITrainingRunRepository _trainingRunRepository;
    private readonly IModelVersionRepository _modelVersionRepository;
    private readonly IDatasetInfoRepository _datasetInfoRepository;
    private readonly string _modelsDirectory;

    public TrainingService(
        IMlService mlService,
        ITrainingRunRepository trainingRunRepository,
        IModelVersionRepository modelVersionRepository,
        IDatasetInfoRepository datasetInfoRepository,
        IConfiguration configuration)
    {
        _mlService = mlService;
        _trainingRunRepository = trainingRunRepository;
        _modelVersionRepository = modelVersionRepository;
        _datasetInfoRepository = datasetInfoRepository;
        _modelsDirectory = configuration["ML:ModelsPath"] ?? "Models";
        
        if (!Directory.Exists(_modelsDirectory))
        {
            Directory.CreateDirectory(_modelsDirectory);
        }
    }

    public async Task<TrainingRunDto> StartTrainingAsync(TrainingRequestDto request, CancellationToken cancellationToken = default)
    {
        // Check if there's already a running training
        var currentRun = await _trainingRunRepository.GetCurrentRunAsync(cancellationToken);
        if (currentRun != null)
        {
            throw new InvalidOperationException("A training run is already in progress.");
        }

        // Get dataset info
        var datasetInfo = await _datasetInfoRepository.GetLatestAsync(cancellationToken);
        if (datasetInfo == null || !File.Exists(datasetInfo.FilePath))
        {
            throw new InvalidOperationException("No dataset found. Please generate or upload a dataset first.");
        }

        // Create training run
        var trainingRun = new TrainingRun
        {
            Algorithm = request.Algorithm,
            DatasetId = datasetInfo.Id,
            Status = TrainingStatus.Queued,
            RequestedBy = request.RequestedBy ?? "System",
            StartedAt = DateTime.UtcNow
        };

        await _trainingRunRepository.AddAsync(trainingRun, cancellationToken);

        // Start training in background
        _ = Task.Run(async () =>
        {
            try
            {
                // Update status to loading
                trainingRun.Status = TrainingStatus.LoadingData;
                await _trainingRunRepository.SaveChangesAsync(cancellationToken);

                // Prepare model path
                var version = $"v{DateTime.UtcNow:yyyyMMdd_HHmmss}";
                var modelPath = Path.Combine(_modelsDirectory, $"{request.Algorithm}_{version}.zip");

                // Train model
                trainingRun.Status = TrainingStatus.Training;
                await _trainingRunRepository.SaveChangesAsync(cancellationToken);

                var result = await _mlService.TrainAsync(
                    datasetInfo.FilePath,
                    request.Algorithm,
                    modelPath,
                    cancellationToken);

                // Save metrics
                var metrics = new ModelMetric
                {
                    MAE = result.Metrics.MAE,
                    MSE = result.Metrics.MSE,
                    RMSE = result.Metrics.RMSE,
                    RSquared = result.Metrics.RSquared,
                    MeanAbsolutePercentageError = result.Metrics.MeanAbsolutePercentageError
                };

                // Create model version
                var modelVersion = new ModelVersion
                {
                    Version = version,
                    Algorithm = request.Algorithm,
                    ModelPath = modelPath,
                    DatasetSize = datasetInfo.RecordCount,
                    TrainingRunId = trainingRun.Id,
                    IsActive = false,
                    Metrics = metrics
                };

                await _modelVersionRepository.AddAsync(modelVersion, cancellationToken);

                // Update training run
                trainingRun.Status = TrainingStatus.Completed;
                trainingRun.CompletedAt = DateTime.UtcNow;
                trainingRun.ModelVersionId = modelVersion.Id;
                trainingRun.DurationSeconds = (int)(trainingRun.CompletedAt - trainingRun.StartedAt).TotalSeconds;
                
                await _trainingRunRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                trainingRun.Status = TrainingStatus.Failed;
                trainingRun.ErrorMessage = ex.Message;
                trainingRun.CompletedAt = DateTime.UtcNow;
                await _trainingRunRepository.SaveChangesAsync(cancellationToken);
            }
        }, cancellationToken);

        return MapToDto(trainingRun);
    }

    public async Task<TrainingRunDto?> GetCurrentStatusAsync(CancellationToken cancellationToken = default)
    {
        var currentRun = await _trainingRunRepository.GetCurrentRunAsync(cancellationToken);
        return currentRun == null ? null : MapToDto(currentRun);
    }

    public async Task<(IEnumerable<TrainingRunDto> Runs, int TotalCount)> GetTrainingHistoryAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var allRuns = await _trainingRunRepository.GetAllAsync(cancellationToken);
        var runsList = allRuns.OrderByDescending(r => r.CreatedAt).ToList();
        
        var totalCount = runsList.Count;
        var pagedRuns = runsList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return (pagedRuns.Select(MapToDto), totalCount);
    }

    public async Task<IEnumerable<TrainingRunDto>> GetLatestRunsAsync(int count = 5, CancellationToken cancellationToken = default)
    {
        var runs = await _trainingRunRepository.GetLatestRunsAsync(count, cancellationToken);
        return runs.Select(MapToDto);
    }

    private static TrainingRunDto MapToDto(TrainingRun run)
    {
        return new TrainingRunDto
        {
            Id = run.Id,
            Algorithm = run.Algorithm,
            Status = run.Status,
            DatasetSize = run.Dataset?.RecordCount ?? 0,
            RequestedBy = run.RequestedBy,
            StartedAt = run.StartedAt,
            CompletedAt = run.CompletedAt,
            DurationSeconds = run.DurationSeconds,
            ErrorMessage = run.ErrorMessage,
            ModelVersionId = run.ModelVersionId,
            CreatedAt = run.CreatedAt
        };
    }
}
