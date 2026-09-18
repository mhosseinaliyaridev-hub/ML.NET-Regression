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

    public DatasetController(
        IDatasetService datasetService,
        ILogger<DatasetController> logger)
    {
        _datasetService = datasetService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a new dataset
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(DatasetInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DatasetInfoDto>> GenerateDataset(
        [FromQuery] int recordCount = 100000,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _datasetService.GenerateDatasetAsync(recordCount, cancellationToken);
            _logger.LogInformation(
                "Dataset generated: Records={Records}, File={File}",
                result.RecordCount, result.FileName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dataset generation failed");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Upload a CSV dataset
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(DatasetInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DatasetInfoDto>> UploadDataset(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded" });
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Only CSV files are allowed" });
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await _datasetService.UploadDatasetAsync(stream, file.FileName, cancellationToken);
            _logger.LogInformation(
                "Dataset uploaded: Records={Records}, File={File}",
                result.RecordCount, result.FileName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dataset upload failed");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get latest dataset info
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DatasetInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DatasetInfoDto>> GetLatestDataset(CancellationToken cancellationToken = default)
    {
        var dataset = await _datasetService.GetLatestDatasetAsync(cancellationToken);
        if (dataset == null) return NotFound(new { message = "No dataset found" });
        return Ok(dataset);
    }

    /// <summary>
    /// Get dataset statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(DatasetStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DatasetStatisticsDto>> GetStatistics(CancellationToken cancellationToken = default)
    {
        try
        {
            var stats = await _datasetService.GetStatisticsAsync(cancellationToken);
            return Ok(stats);
        }
        catch (NotFoundException)
        {
            return NotFound(new { message = "No dataset found" });
        }
    }

    /// <summary>
    /// Get dataset preview with pagination
    /// </summary>
    [HttpGet("preview")]
    [ProducesResponseType(typeof(PagedResponse<StudentInput>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<StudentInput>>> GetPreview(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _datasetService.GetPreviewAsync(pageNumber, pageSize, cancellationToken);
        return Ok(new PagedResponse<StudentInput>(result.Data, result.TotalCount, pageNumber, pageSize));
    }
}
