using StudentScorePrediction.Domain.Entities;

namespace StudentScorePrediction.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Student> Students, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    Task<Student> AddAsync(Student student, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}

public interface IPredictionRepository
{
    Task<Prediction?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prediction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Prediction> Predictions, int TotalCount)> GetPagedAsync(int page, int pageSize, int? studentId = null, CancellationToken cancellationToken = default);
    Task<Prediction> AddAsync(Prediction prediction, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prediction>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}

public interface IModelVersionRepository
{
    Task<ModelVersion?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ModelVersion>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ModelVersion> AddAsync(ModelVersion model, CancellationToken cancellationToken = default);
    Task UpdateAsync(ModelVersion model, CancellationToken cancellationToken = default);
    Task<ModelVersion?> GetActiveAsync(CancellationToken cancellationToken = default);
    Task SetAllInactiveAsync(CancellationToken cancellationToken = default);
}

public interface ITrainingRunRepository
{
    Task<TrainingRun?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TrainingRun>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TrainingRun> AddAsync(TrainingRun run, CancellationToken cancellationToken = default);
    Task UpdateAsync(TrainingRun run, CancellationToken cancellationToken = default);
    Task<TrainingRun?> GetLatestAsync(CancellationToken cancellationToken = default);
}

public interface IDatasetInfoRepository
{
    Task<DatasetInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DatasetInfo?> GetLatestAsync(CancellationToken cancellationToken = default);
    Task<DatasetInfo> AddAsync(DatasetInfo info, CancellationToken cancellationToken = default);
}
