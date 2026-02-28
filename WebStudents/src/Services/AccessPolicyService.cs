using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class AccessPolicyService
{
    private readonly StudentDbContext _context;

    public AccessPolicyService(StudentDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsProfessorOwnerOfOfferingAsync(Guid offeringId, Guid professorId)
    {
        return await _context.DisciplineOfferings
            .AnyAsync(o => o.Id == offeringId && o.ProffessorId == professorId);
    }

    public async Task EnsureProfessorOwnsOfferingAsync(Guid offeringId, Guid? professorId)
    {
        if (!professorId.HasValue)
        {
            throw new ApiException("Не указан LinkedPersonId преподавателя", StatusCodes.Status403Forbidden);
        }

        var isOwner = await IsProfessorOwnerOfOfferingAsync(offeringId, professorId.Value);
        if (!isOwner)
        {
            throw new ApiException("Offering недоступен для этого преподавателя", StatusCodes.Status403Forbidden);
        }
    }

    public async Task EnsureOfferingIsOpenAsync(Guid offeringId)
    {
        var hasClosedSheet = await _context.GradeSheets
            .AnyAsync(s => s.DisciplineOfferingId == offeringId && s.Status == SheetStatus.Closed);

        if (hasClosedSheet)
        {
            throw new ApiException("Ведомость закрыта, изменения запрещены", StatusCodes.Status409Conflict);
        }
    }

    public async Task EnsureSheetIsOpenAsync(Guid sheetId)
    {
        var sheet = await _context.GradeSheets.FirstOrDefaultAsync(s => s.Id == sheetId);
        if (sheet == null)
        {
            throw new ApiException("Ведомость не найдена", StatusCodes.Status404NotFound);
        }

        if (sheet.Status == SheetStatus.Closed)
        {
            throw new ApiException("Ведомость закрыта, изменения запрещены", StatusCodes.Status409Conflict);
        }
    }

    public async Task<Guid> ResolveGradeOfferingAsync(int gradeId)
    {
        var grade = await _context.Grades
            .Include(g => g.Assignment)
            .FirstOrDefaultAsync(g => g.Id == gradeId);

        if (grade == null)
        {
            throw new ApiException("Оценка не найдена", StatusCodes.Status404NotFound);
        }

        var offeringId = grade.DisciplineOfferingId ?? grade.Assignment?.DisciplineOfferingId;
        if (!offeringId.HasValue)
        {
            throw new ApiException("У оценки не задан DisciplineOffering", StatusCodes.Status400BadRequest);
        }

        return offeringId.Value;
    }

    public async Task<Guid> ResolveAssignmentOfferingAsync(int assignmentId)
    {
        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
        if (assignment == null)
        {
            throw new ApiException("Задание не найдено", StatusCodes.Status404NotFound);
        }

        if (!assignment.DisciplineOfferingId.HasValue)
        {
            throw new ApiException("У задания не задан DisciplineOffering", StatusCodes.Status400BadRequest);
        }

        return assignment.DisciplineOfferingId.Value;
    }
}
