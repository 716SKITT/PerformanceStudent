using Microsoft.EntityFrameworkCore;
using WebStudents.src.EF;

namespace WebStudents.UnitTests.Support;

public static class TestDbFactory
{
    public static StudentDbContext CreateContext(string? name = null)
    {
        var options = new DbContextOptionsBuilder<StudentDbContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;

        return new StudentDbContext(options);
    }
}
