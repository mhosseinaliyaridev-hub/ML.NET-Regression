using StudentScorePrediction.Domain.Entities;

namespace StudentScorePrediction.Infrastructure.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetWithPredictionsAsync(int id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Student> Students, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public class StudentRepository : EfRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetWithPredictionsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Predictions)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s => 
                s.Name.Contains(searchTerm) || 
                s.Email!.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var students = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (students, totalCount);
    }
}

public interface IPredictionRepository : IRepository<Prediction>
{
    Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetByStudentIdAsync(
        int studentId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<Prediction> Predictions, int TotalCount)> SearchAsync(
        string? searchTerm,
        int? modelVersionId,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    Task<decimal> GetAveragePredictedScoreAsync(CancellationToken cancellationToken = default);
}

public class PredictionRepository : EfRepository<Prediction>, IPredictionRepository
{
    public PredictionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetByStudentIdAsync(
        int studentId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.StudentId == studentId).AsNoTracking();
        
        var totalCount = await query.CountAsync(cancellationToken);
        var predictions = await query
            .Include(p => p.ModelVersion)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (predictions, totalCount);
    }

    public async Task<(IEnumerable<Prediction> Predictions, int TotalCount)> SearchAsync(
        string? searchTerm,
        int? modelVersionId,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Include(p => p.Student).Include(p => p.ModelVersion);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Student!.Name.Contains(searchTerm));
        }

        if (modelVersionId.HasValue)
        {
            query = query.Where(p => p.ModelVersionId == modelVersionId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= toDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var predictions = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (predictions, totalCount);
    }

    public async Task<decimal> GetAveragePredictedScoreAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AverageAsync(p => (decimal?)p.PredictedScore) ?? 0;
    }
}

public interface IModelVersionRepository : IRepository<ModelVersion>
{
    Task<ModelVersion?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ModelVersion>> GetAllWithMetricsAsync(CancellationToken cancellationToken = default);
    Task ActivateModelAsync(int modelId, CancellationToken cancellationToken = default);
}

public class ModelVersionRepository : EfRepository<ModelVersion>, IModelVersionRepository
{
    public ModelVersionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ModelVersion?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Metrics)
            .FirstOrDefaultAsync(m => m.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<ModelVersion>> GetAllWithMetricsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Metrics)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task ActivateModelAsync(int modelId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        // Deactivate all models
        var allModels = await _dbSet.ToListAsync(cancellationToken);
        foreach (var model in allModels)
        {
            model.IsActive = false;
        }
        
        // Activate selected model
        var selectedModel = await _dbSet.FindAsync(new object[] { modelId }, cancellationToken);
        if (selectedModel != null)
        {
            selectedModel.IsActive = true;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}

public interface ITrainingRunRepository : IRepository<TrainingRun>
{
    Task<IEnumerable<TrainingRun>> GetLatestRunsAsync(int count, CancellationToken cancellationToken = default);
    Task<TrainingRun?> GetCurrentRunAsync(CancellationToken cancellationToken = default);
}

public class TrainingRunRepository : EfRepository<TrainingRun>, ITrainingRunRepository
{
    public TrainingRunRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TrainingRun>> GetLatestRunsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Metrics)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<TrainingRun?> GetCurrentRunAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Status == TrainingStatus.Running, cancellationToken);
    }
}

public interface IDatasetInfoRepository : IRepository<DatasetInfo>
{
    Task<DatasetInfo?> GetLatestAsync(CancellationToken cancellationToken = default);
}

public class DatasetInfoRepository : EfRepository<DatasetInfo>, IDatasetInfoRepository
{
    public DatasetInfoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<DatasetInfo?> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
