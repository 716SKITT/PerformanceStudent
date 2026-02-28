using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class GradeSheetService
{
    private readonly StudentDbContext _context;
    private readonly AccessPolicyService _accessPolicy;

    public GradeSheetService(StudentDbContext context, AccessPolicyService accessPolicy)
    {
        _context = context;
        _accessPolicy = accessPolicy;
    }

    public Task<List<GradeSheet>> GetAllAsync()
    {
        return _context.GradeSheets
            .Include(s => s.DisciplineOffering)
                .ThenInclude(o => o!.Discipline)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public Task<List<GradeSheet>> GetByOfferingAsync(Guid offeringId)
    {
        return _context.GradeSheets
            .Where(s => s.DisciplineOfferingId == offeringId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public Task<GradeSheet?> GetByIdAsync(Guid sheetId)
    {
        return _context.GradeSheets.FirstOrDefaultAsync(s => s.Id == sheetId);
    }

    public async Task<GradeSheet> CreateForOfferingAsync(Guid offeringId)
    {
        await _accessPolicy.EnsureOfferingIsOpenAsync(offeringId);

        var offeringExists = await _context.DisciplineOfferings.AnyAsync(o => o.Id == offeringId);
        if (!offeringExists)
        {
            throw new ApiException("DisciplineOffering не найден", StatusCodes.Status404NotFound);
        }

        var sheet = new GradeSheet
        {
            Id = Guid.NewGuid(),
            DisciplineOfferingId = offeringId,
            CreatedAt = DateTime.UtcNow,
            Status = SheetStatus.Open
        };

        _context.GradeSheets.Add(sheet);
        await _context.SaveChangesAsync();
        return sheet;
    }

    public async Task<GradeSheet> CloseAsync(Guid sheetId)
    {
        var sheet = await _context.GradeSheets.FirstOrDefaultAsync(x => x.Id == sheetId);
        if (sheet == null)
        {
            throw new ApiException("Ведомость не найдена", StatusCodes.Status404NotFound);
        }

        if (sheet.Status == SheetStatus.Closed)
        {
            throw new ApiException("Ведомость уже закрыта", StatusCodes.Status409Conflict);
        }

        sheet.Status = SheetStatus.Closed;
        sheet.ClosedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return sheet;
    }
}
