using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Repositories;
using StudentScorePrediction.ML.Services;

namespace StudentScorePrediction.Application.Services;

public class DatasetService : IDatasetService
{
    private readonly IMlService _mlService;
    private readonly IDatasetInfoRepository _datasetInfoRepository;
    private readonly string _datasetsDirectory;

    public DatasetService(
        IMlService mlService,
        IDatasetInfoRepository datasetInfoRepository,
        IConfiguration configuration)
    {
        _mlService = mlService;
        _datasetInfoRepository = datasetInfoRepository;
        _datasetsDirectory = configuration["ML:DatasetsPath"] ?? "Datasets";
        
        if (!Directory.Exists(_datasetsDirectory))
        {
            Directory.CreateDirectory(_datasetsDirectory);
        }
    }

    public async Task<DatasetInfoDto> GenerateDatasetAsync(int recordCount, CancellationToken cancellationToken = default)
    {
        var fileName = $"student_data_{recordCount}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        var filePath = Path.Combine(_datasetsDirectory, fileName);

        // Generate dataset
        await _mlService.GenerateDatasetAsync(filePath, recordCount, cancellationToken);

        // Get statistics
        var stats = await _mlService.GetDatasetStatisticsAsync(filePath, cancellationToken);

        // Save dataset info
        var datasetInfo = new DatasetInfo
        {
            FileName = fileName,
            FilePath = filePath,
            RecordCount = recordCount,
            FeatureCount = stats.FeatureCount,
            MissingValues = stats.MissingValues,
            HasOutliers = stats.HasOutliers,
            MinFinalScore = stats.MinFinalScore,
            MaxFinalScore = stats.MaxFinalScore,
            AverageFinalScore = stats.AverageFinalScore
        };

        await _datasetInfoRepository.AddAsync(datasetInfo, cancellationToken);

        return MapToDto(datasetInfo, stats);
    }

    public async Task<DatasetInfoDto> UploadDatasetAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_datasetsDirectory, fileName);
        
        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs, cancellationToken);
        }

        // Validate and get statistics
        var stats = await _mlService.GetDatasetStatisticsAsync(filePath, cancellationToken);

        var datasetInfo = new DatasetInfo
        {
            FileName = fileName,
            FilePath = filePath,
            RecordCount = stats.RecordCount,
            FeatureCount = stats.FeatureCount,
            MissingValues = stats.MissingValues,
            HasOutliers = stats.HasOutliers,
            MinFinalScore = stats.MinFinalScore,
            MaxFinalScore = stats.MaxFinalScore,
            AverageFinalScore = stats.AverageFinalScore
        };

        await _datasetInfoRepository.AddAsync(datasetInfo, cancellationToken);

        return MapToDto(datasetInfo, stats);
    }

    public async Task<DatasetInfoDto?> GetLatestDatasetAsync(CancellationToken cancellationToken = default)
    {
        var datasetInfo = await _datasetInfoRepository.GetLatestAsync(cancellationToken);
        if (datasetInfo == null) return null;

        var stats = await _mlService.GetDatasetStatisticsAsync(datasetInfo.FilePath, cancellationToken);
        return MapToDto(datasetInfo, stats);
    }

    public async Task<DatasetStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var datasetInfo = await _datasetInfoRepository.GetLatestAsync(cancellationToken);
        if (datasetInfo == null || !File.Exists(datasetInfo.FilePath))
        {
            throw new NotFoundException("No dataset found");
        }

        return await _mlService.GetDatasetStatisticsAsync(datasetInfo.FilePath, cancellationToken);
    }

    public async Task<(IEnumerable<StudentInput> Data, int TotalCount)> GetPreviewAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var datasetInfo = await _datasetInfoRepository.GetLatestAsync(cancellationToken);
        if (datasetInfo == null || !File.Exists(datasetInfo.FilePath))
        {
            throw new NotFoundException("No dataset found");
        }

        return await _mlService.GetDatasetPreviewAsync(datasetInfo.FilePath, pageNumber, pageSize, cancellationToken);
    }

    private static DatasetInfoDto MapToDto(DatasetInfo info, DatasetStatisticsDto stats)
    {
        return new DatasetInfoDto
        {
            Id = info.Id,
            FileName = info.FileName,
            RecordCount = info.RecordCount,
            FeatureCount = info.FeatureCount,
            MissingValues = info.MissingValues,
            HasOutliers = info.HasOutliers,
            MinFinalScore = info.MinFinalScore,
            MaxFinalScore = info.MaxFinalScore,
            AverageFinalScore = info.AverageFinalScore,
            CreatedAt = info.CreatedAt,
            Statistics = stats
        };
    }
}
