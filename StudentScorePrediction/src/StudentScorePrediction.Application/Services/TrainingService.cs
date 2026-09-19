using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.ML.Services;

namespace StudentScorePrediction.Application.Services;

public class TrainingService : ITrainingService
{
    private readonly IMlService _mlService;
    private readonly IUnitOfWork _unitOfWork;
    private static bool _isTraining = false;
    private static string? _currentStatus;

    public TrainingService(IMlService mlService, IUnitOfWork unitOfWork)
    {
        _mlService = mlService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TrainingRunDto> StartTrainingAsync(string algorithm, CancellationToken cancellationToken = default)
    {
        if (_isTraining)
            throw new InvalidOperationException("Training is already in progress");

        _isTraining = true;
        _currentStatus = "Loading Data";

        var datasetInfo = await _unitOfWork.DatasetInfos.GetAllAsync(cancellationToken);
        var latestDataset = datasetInfo.OrderByDescending(d => d.CreatedAt).FirstOrDefault();
        
        if (latestDataset == null)
            throw new InvalidOperationException("No dataset available for training");

        var run = new TrainingRun
        {
            Algorithm = algorithm,
            DatasetId = latestDataset.Id,
            Status = "Starting",
            StartedAt = DateTime.UtcNow
        };

        await _unitOfWork.TrainingRuns.AddAsync(run, cancellationToken);

        _ = Task.Run(async () =>
        {
            try
            {
                _currentStatus = "Preparing Data";
                run.Status = "Preparing Data";
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _currentStatus = "Training";
                run.Status = "Training";
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var result = await _mlService.TrainModelAsync(algorithm, cancellationToken);

                _currentStatus = "Saving Model";
                run.Status = "Saving Model";
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var modelVersion = new ModelVersion
                {
                    Version = $"v{DateTime.Now:yyyyMMdd.HHmmss}",
                    Algorithm = algorithm,
                    ModelPath = result.ModelPath,
                    MAE = result.Metrics.MAE,
                    MSE = result.Metrics.MSE,
                    RMSE = result.Metrics.RMSE,
                    RSquared = result.Metrics.RSquared,
                    TrainingDuration = result.TrainingDuration,
                    DatasetSize = result.DatasetSize,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = false
                };

                await _unitOfWork.ModelVersions.AddAsync(modelVersion, cancellationToken);

                run.Status = "Completed";
                run.CompletedAt = DateTime.UtcNow;
                run.ModelVersionId = modelVersion.Id;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                run.Status = "Failed";
                run.ErrorMessage = ex.Message;
                run.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                _isTraining = false;
                _currentStatus = null;
            }
        }, cancellationToken);

        return new TrainingRunDto
        {
            Id = run.Id,
            Algorithm = run.Algorithm,
            Status = run.Status,
            StartedAt = run.StartedAt
        };
    }

    public async Task<TrainingStatusDto> GetTrainingStatusAsync(CancellationToken cancellationToken = default)
    {
        return new TrainingStatusDto
        {
            IsTraining = _isTraining,
            CurrentStatus = _currentStatus ?? "Idle"
        };
    }

    public async Task<IEnumerable<TrainingRunDto>> GetAllTrainingRunsAsync(CancellationToken cancellationToken = default)
    {
        var runs = await _unitOfWork.TrainingRuns.GetAllAsync(cancellationToken);
        return runs.Select(r => new TrainingRunDto
        {
            Id = r.Id,
            Algorithm = r.Algorithm,
            Status = r.Status,
            StartedAt = r.StartedAt,
            CompletedAt = r.CompletedAt,
            ErrorMessage = r.ErrorMessage,
            ModelVersionId = r.ModelVersionId
        }).OrderByDescending(r => r.StartedAt);
    }
}
