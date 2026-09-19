using Microsoft.EntityFrameworkCore;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class ModelVersionRepository : Repository<ModelVersion>, IModelVersionRepository
{
    public ModelVersionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ModelVersionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var model = await _dbSet.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        return model == null ? null : MapToDto(model);
    }

    public async Task<IEnumerable<ModelVersionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var models = await _dbSet.OrderByDescending(m => m.TrainingDate).ToListAsync(cancellationToken);
        return models.Select(MapToDto);
    }

    public async Task<ModelVersionDto> CreateAsync(ModelVersionDto dto, CancellationToken cancellationToken = default)
    {
        var model = new ModelVersion
        {
            Version = dto.Version,
            Algorithm = dto.Algorithm,
            TrainingDate = dto.TrainingDate,
            DatasetSize = dto.DatasetSize,
            MAE = dto.MAE,
            MSE = dto.MSE,
            RMSE = dto.RMSE,
            RSquared = dto.RSquared,
            IsActive = false,
            ModelPath = dto.ModelPath,
            TrainingDurationSeconds = dto.TrainingDurationSeconds
        };

        await _dbSet.AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(model);
    }

    public async Task<bool> SetActiveAsync(int id, CancellationToken cancellationToken = default)
    {
        // Deactivate all models first
        var allModels = await _dbSet.ToListAsync(cancellationToken);
        foreach (var model in allModels)
        {
            model.IsActive = false;
        }

        // Activate the selected model
        var model = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (model == null) return false;

        model.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ModelVersionDto?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var model = await _dbSet.FirstOrDefaultAsync(m => m.IsActive, cancellationToken);
        return model == null ? null : MapToDto(model);
    }

    private static ModelVersionDto MapToDto(ModelVersion model)
    {
        return new ModelVersionDto
        {
            Id = model.Id,
            Version = model.Version,
            Algorithm = model.Algorithm,
            TrainingDate = model.TrainingDate,
            DatasetSize = model.DatasetSize,
            MAE = model.MAE,
            MSE = model.MSE,
            RMSE = model.RMSE,
            RSquared = model.RSquared,
            IsActive = model.IsActive,
            ModelPath = model.ModelPath,
            TrainingDurationSeconds = model.TrainingDurationSeconds
        };
    }
}
