using WebStudents.src.EF;
using StudentsPerformance.Models;

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
        return _context.Student.ToList();
    }
    public Student? GetStudentById(Guid id)
    {
        return _context.Student.FirstOrDefault(s => s.Id == id);
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
}
