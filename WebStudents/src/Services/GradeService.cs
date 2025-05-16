using StudentsPerformance.Models;
using WebStudents.src.EF;
using Microsoft.EntityFrameworkCore;

namespace WebStudents.src.Services;

public class GradeService
{
    private readonly StudentDbContext _context;

    public GradeService(StudentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Grade> GetGradesByStudent(Guid studentId)
    {
        return _context.Set<Grade>()
            .Include(g => g.Assignment)
            .Where(g => g.StudentId == studentId)
            .ToList();
    }

    public void AddGrade(Grade grade)
    {
        _context.Set<Grade>().Add(grade);
        _context.SaveChanges();
    }

    public void UpdateGrade(Grade grade)
    {
        _context.Set<Grade>().Update(grade);
        _context.SaveChanges();
    }

    public void DeleteGrade(int id)
    {
        var grade = _context.Set<Grade>().Find(id);
        if (grade != null)
        {
            _context.Set<Grade>().Remove(grade);
            _context.SaveChanges();
        }
    }
}
