using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class AttendanceService
{
    private readonly StudentDbContext _context;

    public AttendanceService(StudentDbContext context)
    {
        _context = context;
    }

    public Task<List<Attendance>> GetByStudentAsync(Guid studentId)
    {
        return _context.Attendances
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public Task<List<Attendance>> GetByOfferingAsync(Guid offeringId)
    {
        return _context.Attendances
            .Where(a => a.DisciplineOfferingId == offeringId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<Attendance> MarkAttendanceAsync(Attendance attendance)
    {
        if (!attendance.DisciplineOfferingId.HasValue)
        {
            throw new ApiException("DisciplineOfferingId обязателен", StatusCodes.Status400BadRequest);
        }

        var exists = await _context.DisciplineOfferings.AnyAsync(o => o.Id == attendance.DisciplineOfferingId.Value);
        if (!exists)
        {
            throw new ApiException("DisciplineOffering не найден", StatusCodes.Status404NotFound);
        }

        var existing = await _context.Attendances.FirstOrDefaultAsync(a =>
            a.DisciplineOfferingId == attendance.DisciplineOfferingId.Value &&
            a.StudentId == attendance.StudentId &&
            a.Date.Date == attendance.Date.Date);

        if (existing != null)
        {
            existing.IsPresent = attendance.IsPresent;
            await _context.SaveChangesAsync();
            return existing;
        }

        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
        return attendance;
    }
}
