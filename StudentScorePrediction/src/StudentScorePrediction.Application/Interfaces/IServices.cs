using StudentScorePrediction.Application.DTOs;

namespace StudentScorePrediction.Application.Interfaces;

public interface IPredictionService
{
    Task<PredictionResultDto> PredictAsync(PredictionRequestDto request, CancellationToken cancellationToken = default);
    Task<(IEnumerable<PredictionDto> Predictions, int TotalCount)> GetAllAsync(
        string? searchTerm = null,
        int? modelVersionId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<PredictionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<decimal> GetAveragePredictedScoreAsync(CancellationToken cancellationToken = default);
}

public interface ITrainingService
{
    Task<TrainingRunDto> StartTrainingAsync(TrainingRequestDto request, CancellationToken cancellationToken = default);
    Task<TrainingRunDto?> GetCurrentStatusAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<TrainingRunDto> Runs, int TotalCount)> GetTrainingHistoryAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<TrainingRunDto>> GetLatestRunsAsync(int count = 5, CancellationToken cancellationToken = default);
}

public interface IModelService
{
    Task<ModelVersionDto?> GetActiveModelAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ModelVersionDto>> GetAllModelsAsync(CancellationToken cancellationToken = default);
    Task<ModelVersionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task ActivateModelAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ModelComparisonDto>> CompareModelsAsync(CancellationToken cancellationToken = default);
}

public interface IDatasetService
{
    Task<DatasetInfoDto> GenerateDatasetAsync(int recordCount, CancellationToken cancellationToken = default);
    Task<DatasetInfoDto> UploadDatasetAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    Task<DatasetInfoDto?> GetLatestDatasetAsync(CancellationToken cancellationToken = default);
    Task<DatasetStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<StudentInput> Data, int TotalCount)> GetPreviewAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}

public interface IStudentService
{
    Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<StudentDto> Students, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<(IEnumerable<StudentDto> Students, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<StudentDto> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PredictionDto>> GetPredictionsAsync(
        int studentId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
