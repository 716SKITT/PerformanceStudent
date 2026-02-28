using StudentsPerformance.Models;
using Microsoft.EntityFrameworkCore;

namespace WebStudents.src.EF;

public static class DbSeeder
{
    public static void Seed(StudentDbContext context)
    {
        if (!context.UserAccounts.Any())
        {
            context.UserAccounts.Add(new UserAccount
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = "admin123",
                Role = "Admin"
            });
        }

        if (context.AcademicYears.Any())
        {
            context.SaveChanges();
            SyncIdentitySequences(context);
            return;
        }

        var year = new AcademicYear
        {
            Id = Guid.NewGuid(),
            StartYear = DateTime.UtcNow.Year,
            EndYear = DateTime.UtcNow.Year + 1
        };

        var semester = new Semester
        {
            Id = Guid.NewGuid(),
            Number = 1,
            AcademicYearId = year.Id
        };

        var group = new StudentGroup
        {
            Id = Guid.NewGuid(),
            Name = "ИС-221/1",
            StudyYear = 2,
            AcademicYearId = year.Id
        };

        var discipline = new Discipline
        {
            Id = Guid.NewGuid(),
            Name = "Базы данных",
            Hours = 72,
            ControlType = ControlType.Exam
        };

        var professor = new Proffessor
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            Gender = Genders.Man,
            EnrollmentDate = DateTime.UtcNow,
            DateOfBirth = DateTime.SpecifyKind(new DateTime(1985, 6, 15), DateTimeKind.Utc)
        };

        var student1 = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Петр",
            LastName = "Петров",
            Gender = Genders.Man,
            EnrollmentDate = DateTime.UtcNow,
            DateOfBirth = DateTime.SpecifyKind(new DateTime(2006, 3, 10), DateTimeKind.Utc),
            StudentGroupId = group.Id
        };

        var student2 = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Анна",
            LastName = "Сидорова",
            Gender = Genders.Woman,
            EnrollmentDate = DateTime.UtcNow,
            DateOfBirth = DateTime.SpecifyKind(new DateTime(2006, 8, 5), DateTimeKind.Utc),
            StudentGroupId = group.Id
        };

        var offering = new DisciplineOffering
        {
            Id = Guid.NewGuid(),
            DisciplineId = discipline.Id,
            SemesterId = semester.Id,
            StudentGroupId = group.Id,
            ProffessorId = professor.Id
        };

        context.AcademicYears.Add(year);
        context.Semesters.Add(semester);
        context.StudentGroups.Add(group);
        context.Disciplines.Add(discipline);
        context.Proffessor.Add(professor);
        context.Student.AddRange(student1, student2);
        context.DisciplineOfferings.Add(offering);

        if (!context.UserAccounts.Any(u => u.Role == "Professor" && u.LinkedPersonId == professor.Id))
        {
            context.UserAccounts.Add(new UserAccount
            {
                Id = Guid.NewGuid(),
                Username = "prof1",
                PasswordHash = "prof123",
                Role = "Professor",
                LinkedPersonId = professor.Id
            });
        }

        if (!context.UserAccounts.Any(u => u.Role == "Student" && u.LinkedPersonId == student1.Id))
        {
            context.UserAccounts.Add(new UserAccount
            {
                Id = Guid.NewGuid(),
                Username = "stud1",
                PasswordHash = "stud123",
                Role = "Student",
                LinkedPersonId = student1.Id
            });
        }

        if (!context.UserAccounts.Any(u => u.Role == "Student" && u.LinkedPersonId == student2.Id))
        {
            context.UserAccounts.Add(new UserAccount
            {
                Id = Guid.NewGuid(),
                Username = "stud2",
                PasswordHash = "stud123",
                Role = "Student",
                LinkedPersonId = student2.Id
            });
        }

        context.SaveChanges();
        SyncIdentitySequences(context);
    }

    private static void SyncIdentitySequences(StudentDbContext context)
    {
        context.Database.ExecuteSqlRaw("""
            SELECT setval(pg_get_serial_sequence('"Course"', 'Id'), GREATEST(COALESCE((SELECT MAX("Id") FROM "Course"), 0), 1), true);
            SELECT setval(pg_get_serial_sequence('"Assignment"', 'Id'), GREATEST(COALESCE((SELECT MAX("Id") FROM "Assignment"), 0), 1), true);
            SELECT setval(pg_get_serial_sequence('"Grade"', 'Id'), GREATEST(COALESCE((SELECT MAX("Id") FROM "Grade"), 0), 1), true);
            SELECT setval(pg_get_serial_sequence('"Attendance"', 'Id'), GREATEST(COALESCE((SELECT MAX("Id") FROM "Attendance"), 0), 1), true);
            """);
    }
}
