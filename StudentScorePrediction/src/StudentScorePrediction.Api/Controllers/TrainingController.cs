using Microsoft.AspNetCore.Mvc;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;

namespace StudentScorePrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainingController : ControllerBase
{
    private readonly ITrainingService _trainingService;
    private readonly ILogger<TrainingController> _logger;

    public TrainingController(
        ITrainingService trainingService,
        ILogger<TrainingController> logger)
    {
        _trainingService = trainingService;
        _logger = logger;
    }

    /// <summary>
    /// Start a new training run
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(TrainingRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<TrainingRunDto>> StartTraining(
        [FromBody] TrainingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _trainingService.StartTrainingAsync(request, cancellationToken);
            _logger.LogInformation(
                "Training started: Algorithm={Algorithm}, RequestedBy={RequestedBy}",
                request.Algorithm, request.RequestedBy);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Training start failed");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get current training status
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(TrainingRunDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainingRunDto>> GetCurrentStatus(CancellationToken cancellationToken = default)
    {
        var status = await _trainingService.GetCurrentStatusAsync(cancellationToken);
        if (status == null) return NotFound(new { message = "No active training run" });
        return Ok(status);
    }

    /// <summary>
    /// Get training history with pagination
    /// </summary>
    [HttpGet("runs")]
    [ProducesResponseType(typeof(PagedResponse<TrainingRunDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TrainingRunDto>>> GetTrainingHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _trainingService.GetTrainingHistoryAsync(pageNumber, pageSize, cancellationToken);
        return Ok(new PagedResponse<TrainingRunDto>(result.Runs, result.TotalCount, pageNumber, pageSize));
    }

    /// <summary>
    /// Get latest training runs
    /// </summary>
    [HttpGet("runs/latest")]
    [ProducesResponseType(typeof(IEnumerable<TrainingRunDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TrainingRunDto>>> GetLatestRuns(
        [FromQuery] int count = 5,
        CancellationToken cancellationToken = default)
    {
        var runs = await _trainingService.GetLatestRunsAsync(count, cancellationToken);
        return Ok(runs);
    }
}
