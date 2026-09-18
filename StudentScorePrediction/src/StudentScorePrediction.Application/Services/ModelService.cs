using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Application.Services;

public class ModelService : IModelService
{
    private readonly IModelVersionRepository _modelVersionRepository;
    private readonly ITrainingRunRepository _trainingRunRepository;

    public ModelService(
        IModelVersionRepository modelVersionRepository,
        ITrainingRunRepository trainingRunRepository)
    {
        _modelVersionRepository = modelVersionRepository;
        _trainingRunRepository = trainingRunRepository;
    }

    public async Task<ModelVersionDto?> GetActiveModelAsync(CancellationToken cancellationToken = default)
    {
        var model = await _modelVersionRepository.GetActiveAsync(cancellationToken);
        if (model == null) return null;

        return MapToDto(model);
    }

    public async Task<IEnumerable<ModelVersionDto>> GetAllModelsAsync(CancellationToken cancellationToken = default)
    {
        var models = await _modelVersionRepository.GetAllWithMetricsAsync(cancellationToken);
        return models.Select(MapToDto);
    }

    public async Task<ModelVersionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var model = await _modelVersionRepository.GetByIdAsync(id, cancellationToken);
        if (model == null) return null;

        return MapToDto(model);
    }

    public async Task ActivateModelAsync(int id, CancellationToken cancellationToken = default)
    {
        var model = await _modelVersionRepository.GetByIdAsync(id, cancellationToken);
        if (model == null)
        {
            throw new NotFoundException($"Model with ID {id} not found");
        }

        await _modelVersionRepository.ActivateModelAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<ModelComparisonDto>> CompareModelsAsync(CancellationToken cancellationToken = default)
    {
        var models = await _modelVersionRepository.GetAllWithMetricsAsync(cancellationToken);
        
        return models.Select(m => new ModelComparisonDto
        {
            Id = m.Id,
            Version = m.Version,
            Algorithm = m.Algorithm,
            TrainingDate = m.CreatedAt,
            DatasetSize = m.DatasetSize,
            MAE = m.Metrics?.MAE ?? 0,
            MSE = m.Metrics?.MSE ?? 0,
            RMSE = m.Metrics?.RMSE ?? 0,
            RSquared = m.Metrics?.RSquared ?? 0,
            IsActive = m.IsActive
        });
    }

    private static ModelVersionDto MapToDto(ModelVersion model)
    {
        return new ModelVersionDto
        {
            Id = model.Id,
            Version = model.Version,
            Algorithm = model.Algorithm,
            ModelPath = model.ModelPath,
            DatasetSize = model.DatasetSize,
            IsActive = model.IsActive,
            CreatedAt = model.CreatedAt,
            Metrics = model.Metrics != null ? new ModelMetricsDto
            {
                MAE = model.Metrics.MAE,
                MSE = model.Metrics.MSE,
                RMSE = model.Metrics.RMSE,
                RSquared = model.Metrics.RSquared,
                MeanAbsolutePercentageError = model.Metrics.MeanAbsolutePercentageError
            } : null
        };
    }
}
