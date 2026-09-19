using StudentScorePrediction.Application.DTOs;

namespace StudentScorePrediction.Application.Interfaces;

public interface IStudentService
{
    Task<StudentDto?> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<StudentDto>> GetPagedStudentsAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<StudentDto?> UpdateStudentAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken = default);
}

public interface IPredictionService
{
    Task<PredictionDto?> GetPredictionByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<PredictionDto>> GetPagedPredictionsAsync(int pageNumber, int pageSize, int? studentId, CancellationToken cancellationToken = default);
    Task<PredictionResultDto> MakePredictionAsync(PredictionRequestDto request, CancellationToken cancellationToken = default);
}

public interface ITrainingService
{
    Task<TrainingRunDto> StartTrainingAsync(string algorithm, CancellationToken cancellationToken = default);
    Task<TrainingStatusDto> GetTrainingStatusAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TrainingRunDto>> GetAllTrainingRunsAsync(CancellationToken cancellationToken = default);
}

public interface IModelService
{
    Task<IEnumerable<ModelVersionDto>> GetAllModelsAsync(CancellationToken cancellationToken = default);
    Task<ModelVersionDto?> GetModelByIdAsync(int id, CancellationToken cancellationToken = default);
    Task ActivateModelAsync(int id, CancellationToken cancellationToken = default);
}

public interface IDatasetService
{
    Task<DatasetInfoDto> GenerateDatasetAsync(int recordCount, CancellationToken cancellationToken = default);
    Task<DatasetInfoDto> UploadDatasetAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
    Task<DatasetStatisticsDto> GetDatasetStatisticsAsync(CancellationToken cancellationToken = default);
}
