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

    public PredictionsController(IPredictionService predictionService, ILogger<PredictionsController> logger)
    {
        _predictionService = predictionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all predictions with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<PredictionDto>>> GetPredictions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? studentId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _predictionService.GetPagedPredictionsAsync(pageNumber, pageSize, studentId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get prediction by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PredictionDto>> GetPrediction(int id, CancellationToken cancellationToken)
    {
        var prediction = await _predictionService.GetPredictionByIdAsync(id, cancellationToken);
        if (prediction == null)
            return NotFound();

        return Ok(prediction);
    }

    /// <summary>
    /// Make a new prediction based on student data
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PredictionResultDto>> Predict([FromBody] PredictionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _predictionService.MakePredictionAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
