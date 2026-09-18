using StudentScorePrediction.Application.DTOs;

namespace StudentScorePrediction.Application.Interfaces;

public interface IPredictionService
{
    Task<PredictionResultDto> PredictAsync(CreatePredictionDto input, CancellationToken cancellationToken = default);
    Task<IEnumerable<PredictionDto>> GetPredictionsAsync(int page, int pageSize, int? studentId = null, CancellationToken cancellationToken = default);
    Task<int> GetTotalPredictionCountAsync(CancellationToken cancellationToken = default);
}

public interface ITrainingService
{
    Task<int> StartTrainingAsync(string algorithm, CancellationToken cancellationToken = default);
    Task<TrainingRunDto?> GetTrainingStatusAsync(int trainingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TrainingRunDto>> GetTrainingRunsAsync(CancellationToken cancellationToken = default);
}

public interface IModelService
{
    Task<IEnumerable<ModelVersionDto>> GetAllModelsAsync(CancellationToken cancellationToken = default);
    Task<ModelVersionDto?> GetModelByIdAsync(int id, CancellationToken cancellationToken = default);
    Task ActivateModelAsync(int id, CancellationToken cancellationToken = default);
    Task<ModelVersionDto?> GetActiveModelAsync(CancellationToken cancellationToken = default);
}

public interface IDatasetService
{
    Task<DatasetStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default);
    Task<string> GenerateDatasetAsync(int recordCount, CancellationToken cancellationToken = default);
    Task<string> UploadDatasetAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}

public interface IStudentService
{
    Task<StudentDto?> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<StudentDto> Students, int TotalCount)> GetStudentsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task UpdateStudentAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task DeleteStudentAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetTotalStudentCountAsync(CancellationToken cancellationToken = default);
}
