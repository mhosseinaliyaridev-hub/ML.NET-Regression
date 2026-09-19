using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.ML.Services;

namespace StudentScorePrediction.Application.Services;

public class PredictionService : IPredictionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMlService _mlService;

    public PredictionService(IUnitOfWork unitOfWork, IMlService mlService)
    {
        _unitOfWork = unitOfWork;
        _mlService = mlService;
    }

    public async Task<PredictionDto?> GetPredictionByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var prediction = await _unitOfWork.Predictions.GetByIdAsync(id, cancellationToken);
        return prediction?.ToDto();
    }

    public async Task<PagedResult<PredictionDto>> GetPagedPredictionsAsync(int pageNumber, int pageSize, int? studentId, CancellationToken cancellationToken = default)
    {
        System.Linq.Expressions.Expression<System.Func<Prediction, bool>>? predicate = null;
        
        if (studentId.HasValue)
        {
            predicate = p => p.StudentId == studentId.Value;
        }

        var (items, totalCount) = await _unitOfWork.Predictions.GetPagedAsync(
            pageNumber, pageSize, predicate, 
            q => q.OrderByDescending(p => p.CreatedAt),
            cancellationToken);

        return new PagedResult<PredictionDto>
        {
            Items = items.Select(p => p.ToDto()),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PredictionResultDto> MakePredictionAsync(PredictionRequestDto request, CancellationToken cancellationToken = default)
    {
        var input = new StudentInput
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

        var result = await _mlService.PredictAsync(input, cancellationToken);
        
        var prediction = new Prediction
        {
            StudentId = request.StudentId,
            PredictedScore = result.PredictedScore,
            ModelVersionId = result.ModelVersionId,
            InputData = System.Text.Json.JsonSerializer.Serialize(request),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Predictions.AddAsync(prediction, cancellationToken);

        return new PredictionResultDto
        {
            PredictedScore = result.PredictedScore,
            ModelVersion = result.ModelVersion,
            PredictionTime = DateTime.UtcNow,
            DurationMs = result.DurationMs
        };
    }
}
