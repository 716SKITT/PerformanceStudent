using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class AssignmentService
{
    private readonly StudentDbContext _context;

    public AssignmentService(StudentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Assignment>> GetAllAsync(Guid? offeringId = null)
    {
        var query = _context.Assignments.AsQueryable();

        if (offeringId.HasValue)
        {
            query = query.Where(x => x.DisciplineOfferingId == offeringId.Value);
        }

        return await query
            .OrderBy(x => x.DueDate)
            .ToListAsync();
    }

    public Task<Assignment?> GetByIdAsync(int id) =>
        _context.Assignments.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Assignment> AddAsync(Assignment assignment)
    {
        if (!assignment.DisciplineOfferingId.HasValue)
        {
            throw new ApiException("DisciplineOfferingId обязателен", StatusCodes.Status400BadRequest);
        }

        var offeringExists = await _context.DisciplineOfferings.AnyAsync(o => o.Id == assignment.DisciplineOfferingId.Value);
        if (!offeringExists)
        {
            throw new ApiException("DisciplineOffering не найден", StatusCodes.Status404NotFound);
        }

        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.Id == id);
        if (assignment == null)
        {
            return false;
        }

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }
}
