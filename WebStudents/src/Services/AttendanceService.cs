using StudentsPerformance.Models;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class AttendanceService
{
    private readonly StudentDbContext _context;

    public AttendanceService(StudentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Attendance> GetByStudent(Guid studentId)
    {
        return _context.Set<Attendance>()
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.Date)
            .ToList();
    }

    public void MarkAttendance(Attendance attendance)
    {
        _context.Set<Attendance>().Add(attendance);
        _context.SaveChanges();
    }
}
