using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Domain.Enums;
using StudentScorePrediction.Infrastructure.Repositories;

namespace StudentScorePrediction.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IPredictionRepository _predictionRepository;

    public StudentService(IStudentRepository studentRepository, IPredictionRepository predictionRepository)
    {
        _studentRepository = studentRepository;
        _predictionRepository = predictionRepository;
    }

    public async Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetWithPredictionsAsync(id, cancellationToken);
        if (student == null) return null;

        return MapToDto(student);
    }

    public async Task<(IEnumerable<StudentDto> Students, int TotalCount)> GetAllAsync(
        int pageNumber = 1, 
        int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var result = await _studentRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return (result.Items.Select(MapToDto), result.TotalCount);
    }

    public async Task<(IEnumerable<StudentDto> Students, int TotalCount)> SearchAsync(
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _studentRepository.SearchAsync(searchTerm, pageNumber, pageSize, cancellationToken);
        return (result.Students.Select(MapToDto), result.TotalCount);
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email,
            Age = dto.Age,
            Gender = dto.Gender,
            StudyHours = dto.StudyHours,
            AttendanceRate = dto.AttendanceRate,
            HomeworkCompletionRate = dto.HomeworkCompletionRate,
            PreviousAverage = dto.PreviousAverage,
            PreviousExamScore = dto.PreviousExamScore,
            MidtermScore = dto.MidtermScore,
            AbsenceDays = dto.AbsenceDays,
            SleepHours = dto.SleepHours,
            ClassParticipation = dto.ClassParticipation,
            MobileUsageHours = dto.MobileUsageHours,
            PracticeTestCount = dto.PracticeTestCount
        };

        await _studentRepository.AddAsync(student, cancellationToken);
        return MapToDto(student);
    }

    public async Task<StudentDto> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (student == null) throw new NotFoundException($"Student with ID {id} not found");

        student.Name = dto.Name ?? student.Name;
        student.Email = dto.Email ?? student.Email;
        student.Age = dto.Age ?? student.Age;
        student.Gender = dto.Gender ?? student.Gender;
        student.StudyHours = dto.StudyHours ?? student.StudyHours;
        student.AttendanceRate = dto.AttendanceRate ?? student.AttendanceRate;
        student.HomeworkCompletionRate = dto.HomeworkCompletionRate ?? student.HomeworkCompletionRate;
        student.PreviousAverage = dto.PreviousAverage ?? student.PreviousAverage;
        student.PreviousExamScore = dto.PreviousExamScore ?? student.PreviousExamScore;
        student.MidtermScore = dto.MidtermScore ?? student.MidtermScore;
        student.AbsenceDays = dto.AbsenceDays ?? student.AbsenceDays;
        student.SleepHours = dto.SleepHours ?? student.SleepHours;
        student.ClassParticipation = dto.ClassParticipation ?? student.ClassParticipation;
        student.MobileUsageHours = dto.MobileUsageHours ?? student.MobileUsageHours;
        student.PracticeTestCount = dto.PracticeTestCount ?? student.PracticeTestCount;

        _studentRepository.Update(student);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(student);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        if (student == null) throw new NotFoundException($"Student with ID {id} not found");

        _studentRepository.Remove(student);
        await _studentRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<PredictionDto>> GetPredictionsAsync(
        int studentId, 
        int pageNumber = 1, 
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _predictionRepository.GetByStudentIdAsync(studentId, pageNumber, pageSize, cancellationToken);
        return result.Predictions.Select(MapToDto);
    }

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age,
            Gender = student.Gender,
            StudyHours = student.StudyHours,
            AttendanceRate = student.AttendanceRate,
            HomeworkCompletionRate = student.HomeworkCompletionRate,
            PreviousAverage = student.PreviousAverage,
            PreviousExamScore = student.PreviousExamScore,
            MidtermScore = student.MidtermScore,
            AbsenceDays = student.AbsenceDays,
            SleepHours = student.SleepHours,
            ClassParticipation = student.ClassParticipation,
            MobileUsageHours = student.MobileUsageHours,
            PracticeTestCount = student.PracticeTestCount,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }

    private static PredictionDto MapToDto(Prediction prediction)
    {
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
}
