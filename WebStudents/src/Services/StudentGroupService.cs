using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class StudentGroupService
{
    private readonly StudentDbContext _context;

    public StudentGroupService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<StudentGroup>> GetAllAsync() => _context.StudentGroups.OrderBy(x => x.Name).ToListAsync();

    public Task<StudentGroup?> GetByIdAsync(Guid id) => _context.StudentGroups.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<StudentGroup> CreateAsync(StudentGroup model)
    {
        _context.StudentGroups.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateAsync(Guid id, StudentGroup model)
    {
        var existing = await _context.StudentGroups.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        existing.Name = model.Name;
        existing.StudyYear = model.StudyYear;
        existing.AcademicYearId = model.AcademicYearId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.StudentGroups.FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return false;

        _context.StudentGroups.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
