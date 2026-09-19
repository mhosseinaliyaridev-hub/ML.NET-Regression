using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.ML.DataGeneration;
using StudentScorePrediction.ML.Services;

namespace StudentScorePrediction.Application.Services;

public class DatasetService : IDatasetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMlService _mlService;

    public DatasetService(IUnitOfWork unitOfWork, IMlService mlService)
    {
        _unitOfWork = unitOfWork;
        _mlService = mlService;
    }

    public async Task<DatasetInfoDto> GenerateDatasetAsync(int recordCount, CancellationToken cancellationToken = default)
    {
        if (recordCount < 1000 || recordCount > 1000000)
            throw new ArgumentException("Record count must be between 1000 and 1000000");

        var filePath = await _mlService.GenerateDatasetAsync(recordCount, cancellationToken);
        
        var datasetInfo = new DatasetInfo
        {
            FileName = Path.GetFileName(filePath),
            FilePath = filePath,
            RecordCount = recordCount,
            Source = "Generated",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.DatasetInfos.AddAsync(datasetInfo, cancellationToken);

        return new DatasetInfoDto
        {
            Id = datasetInfo.Id,
            FileName = datasetInfo.FileName,
            RecordCount = datasetInfo.RecordCount,
            Source = datasetInfo.Source,
            CreatedAt = datasetInfo.CreatedAt
        };
    }

    public async Task<DatasetInfoDto> UploadDatasetAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        var recordCount = await CountCsvRecordsAsync(filePath, cancellationToken);

        var datasetInfo = new DatasetInfo
        {
            FileName = fileName,
            FilePath = filePath,
            RecordCount = recordCount,
            Source = "Uploaded",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.DatasetInfos.AddAsync(datasetInfo, cancellationToken);

        return new DatasetInfoDto
        {
            Id = datasetInfo.Id,
            FileName = datasetInfo.FileName,
            RecordCount = datasetInfo.RecordCount,
            Source = datasetInfo.Source,
            CreatedAt = datasetInfo.CreatedAt
        };
    }

    public async Task<DatasetStatisticsDto> GetDatasetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var datasets = await _unitOfWork.DatasetInfos.GetAllAsync(cancellationToken);
        var latestDataset = datasets.OrderByDescending(d => d.CreatedAt).FirstOrDefault();

        if (latestDataset == null)
        {
            return new DatasetStatisticsDto
            {
                TotalRecords = 0,
                HasData = false
            };
        }

        return new DatasetStatisticsDto
        {
            TotalRecords = latestDataset.RecordCount,
            HasData = true,
            FileName = latestDataset.FileName,
            CreatedAt = latestDataset.CreatedAt
        };
    }

    private async Task<int> CountCsvRecordsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        int count = 0;
        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                await reader.ReadLineAsync();
                count++;
            }
        }
        return Math.Max(0, count - 1);
    }
}
