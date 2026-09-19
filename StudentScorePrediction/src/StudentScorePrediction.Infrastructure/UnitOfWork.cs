using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IStudentRepository? _studentRepository;
    private IPredictionRepository? _predictionRepository;
    private IModelVersionRepository? _modelVersionRepository;
    private ITrainingRunRepository? _trainingRunRepository;
    private IDatasetInfoRepository? _datasetInfoRepository;
    private IModelMetricRepository? _modelMetricRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IStudentRepository Students => 
        _studentRepository ??= new StudentRepository(_context);

    public IPredictionRepository Predictions => 
        _predictionRepository ??= new PredictionRepository(_context);

    public IModelVersionRepository ModelVersions => 
        _modelVersionRepository ??= new ModelVersionRepository(_context);

    public ITrainingRunRepository TrainingRuns => 
        _trainingRunRepository ??= new TrainingRunRepository(_context);

    public IDatasetInfoRepository DatasetInfos => 
        _datasetInfoRepository ??= new DatasetInfoRepository(_context);

    public IModelMetricRepository ModelMetrics => 
        _modelMetricRepository ??= new ModelMetricRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
