using WebStudents.src.EF;
using StudentsPerformance.Models;
using Microsoft.EntityFrameworkCore;

namespace WebStudents.src.Services;

public class ProffessorService
{
    private readonly StudentDbContext _context;

    public ProffessorService(StudentDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Proffessor> GetAll()
    {
        return _context.Proffessor.Include(p => p.Course).ToList();
    }

    public Proffessor? GetById(Guid id)
    {
        return _context.Proffessor.Include(p => p.Course).FirstOrDefault(p => p.Id == id);
    }

    public void Add(Proffessor proffessor)
    {
        _context.Proffessor.Add(proffessor);
        _context.SaveChanges();
    }

    public void Update(Proffessor proffessor)
    {
        _context.Proffessor.Update(proffessor);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var proffessor = _context.Proffessor.Find(id);
        if (proffessor != null)
        {
            _context.Proffessor.Remove(proffessor);
            _context.SaveChanges();
        }
    }
}
