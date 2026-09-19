using Microsoft.EntityFrameworkCore;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class DatasetInfoRepository : Repository<DatasetInfo>, IDatasetInfoRepository
{
    public DatasetInfoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<DatasetDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dataset = await _dbSet.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        return dataset == null ? null : MapToDto(dataset);
    }

    public async Task<IEnumerable<DatasetDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var datasets = await _dbSet.OrderByDescending(d => d.CreatedDate).ToListAsync(cancellationToken);
        return datasets.Select(MapToDto);
    }

    public async Task<DatasetDto> CreateAsync(DatasetDto dto, CancellationToken cancellationToken = default)
    {
        var dataset = new DatasetInfo
        {
            RecordCount = dto.RecordCount,
            FilePath = dto.FilePath,
            CreatedDate = dto.CreatedDate,
            Source = dto.Source
        };

        await _dbSet.AddAsync(dataset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(dataset);
    }

    private static DatasetDto MapToDto(DatasetInfo dataset)
    {
        return new DatasetDto
        {
            Id = dataset.Id,
            RecordCount = dataset.RecordCount,
            FilePath = dataset.FilePath,
            CreatedDate = dataset.CreatedDate,
            Source = dataset.Source
        };
    }
}
