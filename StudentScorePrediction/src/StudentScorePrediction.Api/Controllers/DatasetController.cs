using Microsoft.AspNetCore.Mvc;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;

namespace StudentScorePrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatasetController : ControllerBase
{
    private readonly IDatasetService _datasetService;
    private readonly ILogger<DatasetController> _logger;

    public DatasetController(IDatasetService datasetService, ILogger<DatasetController> logger)
    {
        _datasetService = datasetService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a new dataset with specified record count
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<DatasetInfoDto>> GenerateDataset([FromBody] GenerateDatasetRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var info = await _datasetService.GenerateDatasetAsync(request.RecordCount, cancellationToken);
            return Ok(info);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Upload a CSV dataset
    /// </summary>
    [HttpPost("upload")]
    public async Task<ActionResult<DatasetInfoDto>> UploadDataset(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            using var stream = file.OpenReadStream();
            var info = await _datasetService.UploadDatasetAsync(stream, file.FileName, cancellationToken);
            return Ok(info);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get dataset statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<DatasetStatisticsDto>> GetStatistics(CancellationToken cancellationToken)
    {
        var stats = await _datasetService.GetDatasetStatisticsAsync(cancellationToken);
        return Ok(stats);
    }
}
