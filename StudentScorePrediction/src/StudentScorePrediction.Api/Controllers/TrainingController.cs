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

    public TrainingController(ITrainingService trainingService, ILogger<TrainingController> logger)
    {
        _trainingService = trainingService;
        _logger = logger;
    }

    /// <summary>
    /// Start a new training run
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<TrainingRunDto>> StartTraining([FromBody] TrainingRequestDto? request = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var run = await _trainingService.StartTrainingAsync(request?.Algorithm ?? "Sdca", cancellationToken);
            return Ok(run);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get training status
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<TrainingStatusDto>> GetStatus(CancellationToken cancellationToken)
    {
        var status = await _trainingService.GetTrainingStatusAsync(cancellationToken);
        return Ok(status);
    }

    /// <summary>
    /// Get all training runs
    /// </summary>
    [HttpGet("runs")]
    public async Task<ActionResult<IEnumerable<TrainingRunDto>>> GetTrainingRuns(CancellationToken cancellationToken)
    {
        var runs = await _trainingService.GetAllTrainingRunsAsync(cancellationToken);
        return Ok(runs);
    }
}
