using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;

namespace StudentScorePrediction.Application.Services;

public class ModelService : IModelService
{
    private readonly IUnitOfWork _unitOfWork;

    public ModelService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ModelVersionDto>> GetAllModelsAsync(CancellationToken cancellationToken = default)
    {
        var models = await _unitOfWork.ModelVersions.GetAllAsync(cancellationToken);
        return models.Select(m => new ModelVersionDto
        {
            Id = m.Id,
            Version = m.Version,
            Algorithm = m.Algorithm,
            MAE = m.MAE,
            MSE = m.MSE,
            RMSE = m.RMSE,
            RSquared = m.RSquared,
            TrainingDuration = m.TrainingDuration,
            DatasetSize = m.DatasetSize,
            CreatedAt = m.CreatedAt,
            IsActive = m.IsActive
        }).OrderByDescending(m => m.CreatedAt);
    }

    public async Task<ModelVersionDto?> GetModelByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var model = await _unitOfWork.ModelVersions.GetByIdAsync(id, cancellationToken);
        if (model == null) return null;

        return new ModelVersionDto
        {
            Id = model.Id,
            Version = model.Version,
            Algorithm = model.Algorithm,
            MAE = model.MAE,
            MSE = model.MSE,
            RMSE = model.RMSE,
            RSquared = model.RSquared,
            TrainingDuration = model.TrainingDuration,
            DatasetSize = model.DatasetSize,
            CreatedAt = model.CreatedAt,
            IsActive = model.IsActive
        };
    }

    public async Task ActivateModelAsync(int id, CancellationToken cancellationToken = default)
    {
        var model = await _unitOfWork.ModelVersions.GetByIdAsync(id, cancellationToken);
        if (model == null)
            throw new InvalidOperationException("Model not found");

        var allModels = await _unitOfWork.ModelVersions.GetAllAsync(cancellationToken);
        foreach (var m in allModels)
        {
            m.IsActive = false;
            _unitOfWork.ModelVersions.Update(m);
        }

        model.IsActive = true;
        _unitOfWork.ModelVersions.Update(model);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
