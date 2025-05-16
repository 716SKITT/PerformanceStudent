using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using StudentsPerformance.Models;

namespace WebStudents.src.EF;
public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> options)
       : base(options)
    {
    }
    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<Student> Student { get; set; }
    public DbSet<Proffessor> Proffessor { get; set; }
    public DbSet<Course> Course { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>().ToTable("UserAccount");
        modelBuilder.Entity<Student>().ToTable("Student");
        modelBuilder.Entity<Proffessor>().ToTable("Proffessor");
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Grade>().ToTable("Grade");
        modelBuilder.Entity<Assignment>().ToTable("Assignment");
        modelBuilder.Entity<Attendance>().ToTable("Attendance");
    }
}