using Microsoft.EntityFrameworkCore;
using StudentScorePrediction.Application.DTOs;
using StudentScorePrediction.Application.Interfaces;
using StudentScorePrediction.Domain.Entities;
using StudentScorePrediction.Infrastructure.Data;

namespace StudentScorePrediction.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        return student == null ? null : MapToDto(student);
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var students = await _dbSet
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return students.Select(MapToDto);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = new Student
        {
            Name = dto.Name,
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

        await _dbSet.AddAsync(student, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(student);
    }

    public async Task<StudentDto?> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var student = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (student == null) return null;

        student.Name = dto.Name ?? student.Name;
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

        _context.Update(student);
        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(student);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (student == null) return false;

        _dbSet.Remove(student);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<StudentDto>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var students = await _dbSet
            .Where(s => s.Name.Contains(searchTerm))
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return students.Select(MapToDto);
    }

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
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
            PracticeTestCount = student.PracticeTestCount
        };
    }
}
