using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class AcademicYearService
{
    private readonly StudentDbContext _context;

    public AcademicYearService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<AcademicYear>> GetAllAsync() => _context.AcademicYears.OrderBy(x => x.StartYear).ToListAsync();

    public Task<AcademicYear?> GetByIdAsync(Guid id) => _context.AcademicYears.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<AcademicYear> CreateAsync(AcademicYear model)
    {
        _context.AcademicYears.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateAsync(Guid id, AcademicYear model)
    {
        var existing = await _context.AcademicYears.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        existing.StartYear = model.StartYear;
        existing.EndYear = model.EndYear;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.AcademicYears.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        _context.AcademicYears.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
