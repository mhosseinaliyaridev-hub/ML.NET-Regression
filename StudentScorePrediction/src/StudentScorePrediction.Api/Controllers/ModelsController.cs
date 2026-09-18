using Microsoft.AspNetCore.Mvc;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;

namespace StudentScorePrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelsController : ControllerBase
{
    private readonly IModelService _modelService;
    private readonly ILogger<ModelsController> _logger;

    public ModelsController(
        IModelService modelService,
        ILogger<ModelsController> logger)
    {
        _modelService = modelService;
        _logger = logger;
    }

    /// <summary>
    /// Get active model
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ModelVersionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ModelVersionDto>> GetActiveModel(CancellationToken cancellationToken = default)
    {
        var model = await _modelService.GetActiveModelAsync(cancellationToken);
        if (model == null) return NotFound(new { message = "No active model found" });
        return Ok(model);
    }

    /// <summary>
    /// Get all models
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ModelVersionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ModelVersionDto>>> GetAllModels(CancellationToken cancellationToken = default)
    {
        var models = await _modelService.GetAllModelsAsync(cancellationToken);
        return Ok(models);
    }

    /// <summary>
    /// Get model by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ModelVersionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ModelVersionDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var model = await _modelService.GetByIdAsync(id, cancellationToken);
        if (model == null) return NotFound();
        return Ok(model);
    }

    /// <summary>
    /// Activate a model
    /// </summary>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateModel(int id, CancellationToken cancellationToken = default)
    {
        await _modelService.ActivateModelAsync(id, cancellationToken);
        _logger.LogInformation("Model activated: ModelId={ModelId}", id);
        return Ok(new { message = "Model activated successfully" });
    }

    /// <summary>
    /// Compare all models
    /// </summary>
    [HttpGet("compare")]
    [ProducesResponseType(typeof(IEnumerable<ModelComparisonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ModelComparisonDto>>> CompareModels(CancellationToken cancellationToken = default)
    {
        var comparison = await _modelService.CompareModelsAsync(cancellationToken);
        return Ok(comparison);
    }
}
