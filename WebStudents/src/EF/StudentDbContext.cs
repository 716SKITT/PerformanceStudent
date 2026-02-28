using Microsoft.EntityFrameworkCore;
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

    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<StudentGroup> StudentGroups { get; set; }
    public DbSet<Discipline> Disciplines { get; set; }
    public DbSet<DisciplineOffering> DisciplineOfferings { get; set; }
    public DbSet<GradeSheet> GradeSheets { get; set; }
    public DbSet<FinalGrade> FinalGrades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>().ToTable("UserAccount");
        modelBuilder.Entity<Student>().ToTable("Student");
        modelBuilder.Entity<Proffessor>().ToTable("Proffessor");
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Grade>().ToTable("Grade");
        modelBuilder.Entity<Assignment>().ToTable("Assignment");
        modelBuilder.Entity<Attendance>().ToTable("Attendance");

        modelBuilder.Entity<AcademicYear>().ToTable("AcademicYear");
        modelBuilder.Entity<Semester>().ToTable("Semester");
        modelBuilder.Entity<StudentGroup>().ToTable("StudentGroup");
        modelBuilder.Entity<Discipline>().ToTable("Discipline");
        modelBuilder.Entity<DisciplineOffering>().ToTable("DisciplineOffering");
        modelBuilder.Entity<GradeSheet>().ToTable("GradeSheet");
        modelBuilder.Entity<FinalGrade>().ToTable("FinalGrade");

        modelBuilder.Entity<Student>()
            .HasOne(s => s.StudentGroup)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.StudentGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Course)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Proffessor>()
            .HasOne(p => p.Course)
            .WithMany()
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Semester>()
            .HasOne(s => s.AcademicYear)
            .WithMany(y => y.Semesters)
            .HasForeignKey(s => s.AcademicYearId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentGroup>()
            .HasOne(g => g.AcademicYear)
            .WithMany(y => y.StudentGroups)
            .HasForeignKey(g => g.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DisciplineOffering>()
            .HasOne(o => o.Discipline)
            .WithMany(d => d.DisciplineOfferings)
            .HasForeignKey(o => o.DisciplineId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DisciplineOffering>()
            .HasOne(o => o.Semester)
            .WithMany(s => s.DisciplineOfferings)
            .HasForeignKey(o => o.SemesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DisciplineOffering>()
            .HasOne(o => o.StudentGroup)
            .WithMany(g => g.DisciplineOfferings)
            .HasForeignKey(o => o.StudentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DisciplineOffering>()
            .HasOne(o => o.Proffessor)
            .WithMany(p => p.DisciplineOfferings)
            .HasForeignKey(o => o.ProffessorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.DisciplineOffering)
            .WithMany(o => o.Assignments)
            .HasForeignKey(a => a.DisciplineOfferingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.DisciplineOffering)
            .WithMany(o => o.Attendances)
            .HasForeignKey(a => a.DisciplineOfferingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Grade>()
            .HasOne(g => g.DisciplineOffering)
            .WithMany(o => o.Grades)
            .HasForeignKey(g => g.DisciplineOfferingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<GradeSheet>()
            .HasOne(s => s.DisciplineOffering)
            .WithMany(o => o.GradeSheets)
            .HasForeignKey(s => s.DisciplineOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FinalGrade>()
            .HasOne(f => f.GradeSheet)
            .WithMany(s => s.FinalGrades)
            .HasForeignKey(f => f.GradeSheetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FinalGrade>()
            .HasOne(f => f.Student)
            .WithMany()
            .HasForeignKey(f => f.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DisciplineOffering>()
            .HasIndex(o => new { o.StudentGroupId, o.SemesterId, o.DisciplineId, o.ProffessorId })
            .IsUnique();

        modelBuilder.Entity<Attendance>()
            .HasIndex(a => new { a.DisciplineOfferingId, a.StudentId, a.Date })
            .IsUnique();

        modelBuilder.Entity<Grade>()
            .HasIndex(g => new { g.DisciplineOfferingId, g.StudentId });

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentGroupId);

        modelBuilder.Entity<DisciplineOffering>()
            .HasIndex(o => o.SemesterId);

        modelBuilder.Entity<FinalGrade>()
            .HasIndex(f => new { f.GradeSheetId, f.StudentId })
            .IsUnique();
    }
}
