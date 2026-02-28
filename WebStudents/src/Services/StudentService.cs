using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.Dtos;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class StudentService
{
    private readonly StudentDbContext _context;

    public StudentService(StudentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Student> GetAllStudents()
    {
        return _context.Student
            .Include(s => s.StudentGroup)
            .Include(s => s.Course)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToList();
    }

    public Student? GetStudentById(Guid id)
    {
        return _context.Student
            .Include(s => s.StudentGroup)
            .Include(s => s.Course)
            .FirstOrDefault(s => s.Id == id);
    }

    public void AddStudent(Student student)
    {
        _context.Student.Add(student);
        _context.SaveChanges();
    }

    public void UpdateStudent(Student student)
    {
        _context.Student.Update(student);
        _context.SaveChanges();
    }

    public void DeleteStudent(Guid id)
    {
        var student = _context.Student.Find(id);
        if (student != null)
        {
            _context.Student.Remove(student);
            _context.SaveChanges();
        }
    }

    public async Task<StudentSummaryDto?> GetStudentSummaryAsync(Guid studentId)
    {
        var student = await _context.Student.FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null) return null;

        var finals = await _context.FinalGrades
            .Where(f => f.StudentId == studentId)
            .Include(f => f.GradeSheet)
                .ThenInclude(s => s!.DisciplineOffering)
                    .ThenInclude(o => o!.Discipline)
            .Include(f => f.GradeSheet)
                .ThenInclude(s => s!.DisciplineOffering)
                    .ThenInclude(o => o!.Semester)
                        .ThenInclude(se => se!.AcademicYear)
            .Select(f => new FinalGradeSummaryItemDto
            {
                GradeSheetId = f.GradeSheetId,
                DisciplineName = f.GradeSheet!.DisciplineOffering!.Discipline!.Name,
                SemesterNumber = f.GradeSheet.DisciplineOffering!.Semester!.Number,
                AcademicYearStart = f.GradeSheet.DisciplineOffering.Semester!.AcademicYear!.StartYear,
                AcademicYearEnd = f.GradeSheet.DisciplineOffering.Semester!.AcademicYear!.EndYear,
                FinalScore = f.FinalScore,
                FinalMark = f.FinalMark,
                UpdatedAt = f.UpdatedAt
            })
            .OrderBy(x => x.AcademicYearStart)
            .ThenBy(x => x.SemesterNumber)
            .ToListAsync();

        return new StudentSummaryDto
        {
            StudentId = student.Id,
            FullName = $"{student.LastName} {student.FirstName}",
            Finals = finals
        };
    }
}
