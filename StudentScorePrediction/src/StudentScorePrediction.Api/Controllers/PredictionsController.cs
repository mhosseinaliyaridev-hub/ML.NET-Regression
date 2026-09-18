using Microsoft.AspNetCore.Mvc;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;

namespace StudentScorePrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredictionsController : ControllerBase
{
    private readonly IPredictionService _predictionService;
    private readonly ILogger<PredictionsController> _logger;

    public PredictionsController(
        IPredictionService predictionService,
        ILogger<PredictionsController> logger)
    {
        _predictionService = predictionService;
        _logger = logger;
    }

    /// <summary>
    /// Make a prediction for a student
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PredictionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PredictionResultDto>> Predict(
        [FromBody] PredictionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _predictionService.PredictAsync(request, cancellationToken);
            _logger.LogInformation(
                "Prediction made: Student={StudentId}, Score={Score}, Model={ModelVersion}",
                result.StudentId, result.PredictedScore, result.ModelVersion);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Prediction failed: No active model");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all predictions with filtering and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PredictionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PredictionDto>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? modelVersionId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _predictionService.GetAllAsync(
            searchTerm, modelVersionId, fromDate, toDate, pageNumber, pageSize, cancellationToken);
        
        return Ok(new PagedResponse<PredictionDto>(result.Predictions, result.TotalCount, pageNumber, pageSize));
    }

    /// <summary>
    /// Get prediction by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PredictionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PredictionDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var prediction = await _predictionService.GetByIdAsync(id, cancellationToken);
        if (prediction == null) return NotFound();
        return Ok(prediction);
    }

    /// <summary>
    /// Get average predicted score
    /// </summary>
    [HttpGet("stats/average")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public async Task<ActionResult<decimal>> GetAveragePredictedScore(CancellationToken cancellationToken = default)
    {
        var average = await _predictionService.GetAveragePredictedScoreAsync(cancellationToken);
        return Ok(average);
    }
}
