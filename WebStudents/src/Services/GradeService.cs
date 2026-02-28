using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class GradeService
{
    private readonly StudentDbContext _context;

    public GradeService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<Grade>> GetGradesByStudentAsync(Guid studentId)
    {
        return _context.Grades
            .Include(g => g.Assignment)
            .Where(g => g.StudentId == studentId)
            .ToListAsync();
    }

    public Task<List<Grade>> GetByOfferingAsync(Guid offeringId)
    {
        return _context.Grades
            .Where(g => g.DisciplineOfferingId == offeringId)
            .OrderBy(g => g.StudentId)
            .ToListAsync();
    }

    public async Task<Grade> AddGradeAsync(Grade grade)
    {
        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == grade.AssignmentId);
        if (assignment == null)
        {
            throw new ApiException("Задание не найдено", StatusCodes.Status404NotFound);
        }

        grade.DisciplineOfferingId = grade.DisciplineOfferingId ?? assignment.DisciplineOfferingId;
        if (!grade.DisciplineOfferingId.HasValue)
        {
            throw new ApiException("DisciplineOfferingId отсутствует у оценки/задания", StatusCodes.Status400BadRequest);
        }

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();
        return grade;
    }

    public async Task<bool> UpdateGradeAsync(Grade grade)
    {
        var existing = await _context.Grades.FirstOrDefaultAsync(g => g.Id == grade.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Score = grade.Score;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGradeAsync(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null)
        {
            return false;
        }

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();
        return true;
    }
}
