using Microsoft.EntityFrameworkCore;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class TrainingRunRepository : Repository<TrainingRun>, ITrainingRunRepository
{
    public TrainingRunRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TrainingRunDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var run = await _dbSet.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return run == null ? null : MapToDto(run);
    }

    public async Task<IEnumerable<TrainingRunDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var runs = await _dbSet.OrderByDescending(r => r.StartTime).ToListAsync(cancellationToken);
        return runs.Select(MapToDto);
    }

    public async Task<TrainingRunDto> CreateAsync(TrainingRunDto dto, CancellationToken cancellationToken = default)
    {
        var run = new TrainingRun
        {
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = dto.Status,
            DatasetSize = dto.DatasetSize,
            Algorithm = dto.Algorithm,
            MAE = dto.MAE,
            MSE = dto.MSE,
            RMSE = dto.RMSE,
            RSquared = dto.RSquared,
            ErrorMessage = dto.ErrorMessage
        };

        await _dbSet.AddAsync(run, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(run);
    }

    public async Task<TrainingRunDto> UpdateAsync(TrainingRunDto dto, CancellationToken cancellationToken = default)
    {
        var run = await _dbSet.FindAsync(new object[] { dto.Id }, cancellationToken);
        if (run == null) throw new InvalidOperationException($"Training run with ID {dto.Id} not found");

        run.EndTime = dto.EndTime;
        run.Status = dto.Status;
        run.MAE = dto.MAE;
        run.MSE = dto.MSE;
        run.RMSE = dto.RMSE;
        run.RSquared = dto.RSquared;
        run.ErrorMessage = dto.ErrorMessage;

        _context.Update(run);
        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(run);
    }

    private static TrainingRunDto MapToDto(TrainingRun run)
    {
        return new TrainingRunDto
        {
            Id = run.Id,
            StartTime = run.StartTime,
            EndTime = run.EndTime,
            Status = run.Status,
            DatasetSize = run.DatasetSize,
            Algorithm = run.Algorithm,
            MAE = run.MAE,
            MSE = run.MSE,
            RMSE = run.RMSE,
            RSquared = run.RSquared,
            ErrorMessage = run.ErrorMessage
        };
    }
}
