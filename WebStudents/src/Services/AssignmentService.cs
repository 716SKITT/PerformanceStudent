using StudentsPerformance.Models;
using WebStudents.src.EF;

namespace WebStudents.src.Services;

public class AssignmentService
{
    private readonly StudentDbContext _context;

    public AssignmentService(StudentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Assignment> GetAll() =>
        _context.Set<Assignment>().ToList();

    public Assignment? GetById(int id) =>
        _context.Set<Assignment>().Find(id);

    public void Add(Assignment assignment)
    {
        _context.Set<Assignment>().Add(assignment);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var assignment = _context.Set<Assignment>().Find(id);
        if (assignment != null)
        {
            _context.Set<Assignment>().Remove(assignment);
            _context.SaveChanges();
        }
    }
}
