using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class SemesterService
{
    private readonly StudentDbContext _context;

    public SemesterService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<Semester>> GetAllAsync() => _context.Semesters.OrderBy(x => x.Number).ToListAsync();

    public Task<Semester?> GetByIdAsync(Guid id) => _context.Semesters.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Semester> CreateAsync(Semester model)
    {
        _context.Semesters.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateAsync(Guid id, Semester model)
    {
        var existing = await _context.Semesters.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        existing.Number = model.Number;
        existing.AcademicYearId = model.AcademicYearId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Semesters.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        _context.Semesters.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
