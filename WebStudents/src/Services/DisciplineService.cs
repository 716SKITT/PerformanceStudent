using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class DisciplineService
{
    private readonly StudentDbContext _context;

    public DisciplineService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<Discipline>> GetAllAsync() => _context.Disciplines.OrderBy(x => x.Name).ToListAsync();

    public Task<Discipline?> GetByIdAsync(Guid id) => _context.Disciplines.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Discipline> CreateAsync(Discipline model)
    {
        _context.Disciplines.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateAsync(Guid id, Discipline model)
    {
        var existing = await _context.Disciplines.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        existing.Name = model.Name;
        existing.Hours = model.Hours;
        existing.ControlType = model.ControlType;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Disciplines.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        _context.Disciplines.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
