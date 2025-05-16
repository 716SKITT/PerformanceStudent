using WebStudents.src.EF;
using StudentsPerformance.Models;

namespace WebStudents.src.Services;
public class AuthService
{
    private readonly StudentDbContext _context;

    public AuthService(StudentDbContext context)
    {
        _context = context;
    }

    public UserAccount? Authenticate(string username, string password)
    {
        var hash = password; // TODO: сделать hashing
        return _context.UserAccounts.FirstOrDefault(u => u.Username == username && u.PasswordHash == hash);
    }

    public void Register(UserAccount user)
    {
        _context.UserAccounts.Add(user);
        _context.SaveChanges();
    }
}
