using WebStudents.src.EF;
using StudentsPerformance.Models;
using Microsoft.EntityFrameworkCore;

namespace WebStudents.src.Services;
public class CourseService
{
    private readonly StudentDbContext _context;

    public CourseService(StudentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Course> GetAllCourses() => _context.Course.ToList();

    public Course? GetCourseById(int id) =>
        _context.Course
            .Include(c => c.Students)
            .FirstOrDefault(c => c.Id == id);

    public void AddCourse(Course course)
    {
        _context.Course.Add(course);
        _context.SaveChanges();
    }

    public void DeleteCourse(int id)
    {
        var course = _context.Course.Find(id);
        if (course != null)
        {
            _context.Course.Remove(course);
            _context.SaveChanges();
        }
    }
}
