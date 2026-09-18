using Microsoft.AspNetCore.Mvc;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;

namespace StudentScorePrediction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all students with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<StudentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<StudentDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _studentService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(new PagedResponse<StudentDto>(result.Students, result.TotalCount, pageNumber, pageSize));
    }

    /// <summary>
    /// Search students by name or email
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResponse<StudentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<StudentDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _studentService.SearchAsync(searchTerm, pageNumber, pageSize, cancellationToken);
        return Ok(new PagedResponse<StudentDto>(result.Students, result.TotalCount, pageNumber, pageSize));
    }

    /// <summary>
    /// Get student by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        if (student == null) return NotFound();
        return Ok(student);
    }

    /// <summary>
    /// Get predictions for a student
    /// </summary>
    [HttpGet("{id}/predictions")]
    [ProducesResponseType(typeof(PagedResponse<PredictionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PredictionDto>>> GetPredictions(
        int id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var predictions = await _studentService.GetPredictionsAsync(id, pageNumber, pageSize, cancellationToken);
        return Ok(new PagedResponse<PredictionDto>(predictions, predictions.Count(), pageNumber, pageSize));
    }

    /// <summary>
    /// Create a new student
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentDto>> Create(
        [FromBody] CreateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentService.CreateAsync(dto, cancellationToken);
        _logger.LogInformation("Student created: {StudentId}", student.Id);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    /// <summary>
    /// Update an existing student
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentDto>> Update(
        int id,
        [FromBody] UpdateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentService.UpdateAsync(id, dto, cancellationToken);
        _logger.LogInformation("Student updated: {StudentId}", student.Id);
        return Ok(student);
    }

    /// <summary>
    /// Delete a student
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        await _studentService.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Student deleted: {StudentId}", id);
        return NoContent();
    }
}
