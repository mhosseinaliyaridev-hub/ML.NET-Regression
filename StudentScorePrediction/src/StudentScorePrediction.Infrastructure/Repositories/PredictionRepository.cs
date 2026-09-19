using Microsoft.EntityFrameworkCore;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class PredictionRepository : Repository<Prediction>, IPredictionRepository
{
    public PredictionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PredictionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var prediction = await _dbSet
            .Include(p => p.Student)
            .Include(p => p.ModelVersion)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        
        return prediction == null ? null : MapToDto(prediction);
    }

    public async Task<IEnumerable<PredictionDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var predictions = await _dbSet
            .Include(p => p.Student)
            .Include(p => p.ModelVersion)
            .OrderByDescending(p => p.PredictionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return predictions.Select(MapToDto);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task<PredictionDto> CreateAsync(CreatePredictionDto dto, CancellationToken cancellationToken = default)
    {
        var prediction = new Prediction
        {
            StudentId = dto.StudentId,
            ModelVersionId = dto.ModelVersionId,
            PredictedScore = dto.PredictedScore,
            ActualScore = dto.ActualScore,
            PredictionDurationMs = dto.PredictionDurationMs,
            PredictionDate = DateTime.UtcNow
        };

        await _dbSet.AddAsync(prediction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        var created = await _dbSet
            .Include(p => p.Student)
            .Include(p => p.ModelVersion)
            .FirstOrDefaultAsync(p => p.Id == prediction.Id, cancellationToken);
        
        return created == null 
            ? throw new InvalidOperationException("Failed to create prediction") 
            : MapToDto(created);
    }

    public async Task<IEnumerable<PredictionDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var predictions = await _dbSet
            .Include(p => p.Student)
            .Include(p => p.ModelVersion)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.PredictionDate)
            .ToListAsync(cancellationToken);
        
        return predictions.Select(MapToDto);
    }

    private static PredictionDto MapToDto(Prediction prediction)
    {
        return new PredictionDto
        {
            Id = prediction.Id,
            StudentId = prediction.StudentId,
            StudentName = prediction.Student?.Name,
            ModelVersionId = prediction.ModelVersionId,
            ModelVersion = prediction.ModelVersion?.Version,
            PredictedScore = prediction.PredictedScore,
            ActualScore = prediction.ActualScore,
            PredictionDate = prediction.PredictionDate,
            PredictionDurationMs = prediction.PredictionDurationMs
        };
    }
}
