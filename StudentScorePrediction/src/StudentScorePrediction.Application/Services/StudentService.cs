using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.ML.Services;

namespace StudentScorePrediction.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
        return student?.ToDto();
    }

    public async Task<PagedResult<StudentDto>> GetPagedStudentsAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken = default)
    {
        System.Linq.Expressions.Expression<System.Func<Student, bool>>? predicate = null;
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            predicate = s => s.FirstName.Contains(searchTerm) || s.LastName.Contains(searchTerm);
        }

        var (items, totalCount) = await _unitOfWork.Students.GetPagedAsync(
            pageNumber, pageSize, predicate, 
            q => q.OrderByDescending(s => s.CreatedAt),
            cancellationToken);

        return new PagedResult<StudentDto>
        {
            Items = items.Select(s => s.ToDto()),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = new Student
        {
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
            PracticeTestCount = dto.PracticeTestCount,
            FirstName = dto.FirstName ?? "Student",
            LastName = dto.LastName ?? "User",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Students.AddAsync(student, cancellationToken);
        return student.ToDto();
    }

    public async Task<StudentDto?> UpdateStudentAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
        if (student == null) return null;

        student.Age = dto.Age;
        student.Gender = dto.Gender;
        student.StudyHours = dto.StudyHours;
        student.AttendanceRate = dto.AttendanceRate;
        student.HomeworkCompletionRate = dto.HomeworkCompletionRate;
        student.PreviousAverage = dto.PreviousAverage;
        student.PreviousExamScore = dto.PreviousExamScore;
        student.MidtermScore = dto.MidtermScore;
        student.AbsenceDays = dto.AbsenceDays;
        student.SleepHours = dto.SleepHours;
        student.ClassParticipation = dto.ClassParticipation;
        student.MobileUsageHours = dto.MobileUsageHours;
        student.PracticeTestCount = dto.PracticeTestCount;

        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return student.ToDto();
    }

    public async Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(id, cancellationToken);
        if (student == null) return false;

        _unitOfWork.Students.Remove(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
