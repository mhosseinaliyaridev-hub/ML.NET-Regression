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

    public ModelsController(IModelService modelService, ILogger<ModelsController> logger)
    {
        _modelService = modelService;
        _logger = logger;
    }

    /// <summary>
    /// Get all model versions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModelVersionDto>>> GetModels(CancellationToken cancellationToken)
    {
        var models = await _modelService.GetAllModelsAsync(cancellationToken);
        return Ok(models);
    }

    /// <summary>
    /// Get model by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ModelVersionDto>> GetModel(int id, CancellationToken cancellationToken)
    {
        var model = await _modelService.GetModelByIdAsync(id, cancellationToken);
        if (model == null)
            return NotFound();

        return Ok(model);
    }

    /// <summary>
    /// Activate a specific model version
    /// </summary>
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateModel(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _modelService.ActivateModelAsync(id, cancellationToken);
            return Ok(new { message = "Model activated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
