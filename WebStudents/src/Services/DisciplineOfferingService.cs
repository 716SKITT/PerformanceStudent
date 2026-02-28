using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class DisciplineOfferingService
{
    private readonly StudentDbContext _context;

    public DisciplineOfferingService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<DisciplineOffering>> GetAllAsync()
    {
        return _context.DisciplineOfferings
            .Include(x => x.Discipline)
            .Include(x => x.Semester)
                .ThenInclude(s => s!.AcademicYear)
            .Include(x => x.StudentGroup)
            .Include(x => x.Proffessor)
            .ToListAsync();
    }

    public Task<List<DisciplineOffering>> GetMyAsync(Guid professorId)
    {
        return _context.DisciplineOfferings
            .Include(x => x.Discipline)
            .Include(x => x.Semester)
                .ThenInclude(s => s!.AcademicYear)
            .Include(x => x.StudentGroup)
            .Where(x => x.ProffessorId == professorId)
            .OrderBy(x => x.Semester!.AcademicYear!.StartYear)
            .ThenBy(x => x.Semester!.Number)
            .ToListAsync();
    }

    public Task<DisciplineOffering?> GetByIdAsync(Guid id)
    {
        return _context.DisciplineOfferings
            .Include(x => x.Discipline)
            .Include(x => x.Semester)
            .Include(x => x.StudentGroup)
            .Include(x => x.Proffessor)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<DisciplineOffering> CreateAsync(DisciplineOffering model)
    {
        _context.DisciplineOfferings.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateAsync(Guid id, DisciplineOffering model)
    {
        var existing = await _context.DisciplineOfferings.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        existing.DisciplineId = model.DisciplineId;
        existing.SemesterId = model.SemesterId;
        existing.StudentGroupId = model.StudentGroupId;
        existing.ProffessorId = model.ProffessorId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.DisciplineOfferings.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        _context.DisciplineOfferings.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
