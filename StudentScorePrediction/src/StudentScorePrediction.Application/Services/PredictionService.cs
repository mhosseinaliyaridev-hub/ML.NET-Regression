using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Application.Services;

public class PredictionService : IPredictionService
{
    private readonly IMlService _mlService;
    private readonly IPredictionRepository _predictionRepository;
    private readonly IModelVersionRepository _modelVersionRepository;
    private readonly IStudentRepository _studentRepository;

    public PredictionService(
        IMlService mlService,
        IPredictionRepository predictionRepository,
        IModelVersionRepository modelVersionRepository,
        IStudentRepository studentRepository)
    {
        _mlService = mlService;
        _predictionRepository = predictionRepository;
        _modelVersionRepository = modelVersionRepository;
        _studentRepository = studentRepository;
    }

    public async Task<PredictionResultDto> PredictAsync(PredictionRequestDto request, CancellationToken cancellationToken = default)
    {
        // Get active model
        var activeModel = await _modelVersionRepository.GetActiveAsync(cancellationToken);
        if (activeModel == null)
        {
            throw new InvalidOperationException("No active model found. Please train and activate a model first.");
        }

        // Create student input
        var studentInput = new StudentInput
        {
            Age = request.Age,
            Gender = request.Gender,
            StudyHours = request.StudyHours,
            AttendanceRate = request.AttendanceRate,
            HomeworkCompletionRate = request.HomeworkCompletionRate,
            PreviousAverage = request.PreviousAverage,
            PreviousExamScore = request.PreviousExamScore,
            MidtermScore = request.MidtermScore,
            AbsenceDays = request.AbsenceDays,
            SleepHours = request.SleepHours,
            ClassParticipation = request.ClassParticipation,
            MobileUsageHours = request.MobileUsageHours,
            PracticeTestCount = request.PracticeTestCount
        };

        // Make prediction
        var startTime = DateTime.UtcNow;
        var predictionResult = await _mlService.PredictAsync(studentInput, activeModel.ModelPath, cancellationToken);
        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

        // Create or get student
        Student? student = null;
        if (request.StudentId.HasValue)
        {
            student = await _studentRepository.GetByIdAsync(request.StudentId.Value, cancellationToken);
        }

        if (student == null && request.StudentId.HasValue)
        {
            throw new NotFoundException($"Student with ID {request.StudentId.Value} not found");
        }

        // Save prediction
        var prediction = new Prediction
        {
            StudentId = student?.Id,
            PredictedScore = Math.Clamp(predictionResult.PredictedScore, 0, 20),
            ActualScore = request.ActualScore,
            ModelVersionId = activeModel.Id,
            DurationMs = (int)duration,
            InputData = System.Text.Json.JsonSerializer.Serialize(studentInput)
        };

        await _predictionRepository.AddAsync(prediction, cancellationToken);

        return new PredictionResultDto
        {
            Id = prediction.Id,
            StudentId = student?.Id,
            StudentName = student?.Name,
            PredictedScore = prediction.PredictedScore,
            Confidence = predictionResult.Confidence,
            ModelVersion = activeModel.Version,
            ModelVersionId = activeModel.Id,
            Algorithm = activeModel.Algorithm,
            DurationMs = (int)duration,
            CreatedAt = prediction.CreatedAt,
            FeatureImportance = predictionResult.FeatureImportance
        };
    }

    public async Task<(IEnumerable<PredictionDto> Predictions, int TotalCount)> GetAllAsync(
        string? searchTerm = null,
        int? modelVersionId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _predictionRepository.SearchAsync(
            searchTerm, modelVersionId, fromDate, toDate, pageNumber, pageSize, cancellationToken);
        
        return (result.Predictions.Select(p => new PredictionDto
        {
            Id = p.Id,
            StudentId = p.StudentId,
            StudentName = p.Student?.Name,
            PredictedScore = p.PredictedScore,
            ActualScore = p.ActualScore,
            ModelVersionId = p.ModelVersionId,
            ModelVersion = p.ModelVersion?.Version,
            Algorithm = p.ModelVersion?.Algorithm,
            DurationMs = p.DurationMs,
            CreatedAt = p.CreatedAt
        }), result.TotalCount);
    }

    public async Task<PredictionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var prediction = await _predictionRepository.GetByIdAsync(id, cancellationToken);
        if (prediction == null) return null;

        return new PredictionDto
        {
            Id = prediction.Id,
            StudentId = prediction.StudentId,
            StudentName = prediction.Student?.Name,
            PredictedScore = prediction.PredictedScore,
            ActualScore = prediction.ActualScore,
            ModelVersionId = prediction.ModelVersionId,
            ModelVersion = prediction.ModelVersion?.Version,
            Algorithm = prediction.ModelVersion?.Algorithm,
            DurationMs = prediction.DurationMs,
            CreatedAt = prediction.CreatedAt
        };
    }

    public async Task<decimal> GetAveragePredictedScoreAsync(CancellationToken cancellationToken = default)
    {
        return await _predictionRepository.GetAveragePredictedScoreAsync(cancellationToken);
    }
}
