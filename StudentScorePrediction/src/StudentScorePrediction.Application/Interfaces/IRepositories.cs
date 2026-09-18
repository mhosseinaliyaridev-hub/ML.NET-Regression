using StudentScorePrediction.Application.DTOs;

namespace StudentScorePrediction.Application.Interfaces;

public interface IStudentRepository
{
    Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<StudentDto?> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentDto>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
}

public interface IPredictionRepository
{
    Task<PredictionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PredictionDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<PredictionDto> CreateAsync(CreatePredictionDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<PredictionDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
}

public interface IModelVersionRepository
{
    Task<ModelVersionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ModelVersionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ModelVersionDto> CreateAsync(ModelVersionDto dto, CancellationToken cancellationToken = default);
    Task<bool> SetActiveAsync(int id, CancellationToken cancellationToken = default);
    Task<ModelVersionDto?> GetActiveAsync(CancellationToken cancellationToken = default);
}

public interface ITrainingRunRepository
{
    Task<TrainingRunDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TrainingRunDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TrainingRunDto> CreateAsync(TrainingRunDto dto, CancellationToken cancellationToken = default);
    Task<TrainingRunDto> UpdateAsync(TrainingRunDto dto, CancellationToken cancellationToken = default);
}

public interface IDatasetRepository
{
    Task<DatasetDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DatasetDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DatasetDto> CreateAsync(DatasetDto dto, CancellationToken cancellationToken = default);
    Task<DatasetStatisticsDto> GetStatisticsAsync(string filePath, CancellationToken cancellationToken = default);
}
