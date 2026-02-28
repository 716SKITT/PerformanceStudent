using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.Services;
using WebStudents.UnitTests.Support;
using Xunit;

namespace WebStudents.UnitTests.Services;

public class FinalGradeServiceTests
{
    [Fact]
    public async Task RecalculateAsync_ShouldThrow_WhenSheetClosed()
    {
        await using var db = TestDbFactory.CreateContext();
        var sheetId = Guid.NewGuid();
        var year = new AcademicYear { Id = Guid.NewGuid(), StartYear = 2025, EndYear = 2026 };
        var semester = new Semester { Id = Guid.NewGuid(), Number = 1, AcademicYearId = year.Id, AcademicYear = year };
        var group = new StudentGroup { Id = Guid.NewGuid(), Name = "ИС-221", StudyYear = 2, AcademicYearId = year.Id, AcademicYear = year };
        var discipline = new Discipline { Id = Guid.NewGuid(), Name = "БД", Hours = 72, ControlType = ControlType.Exam };
        var professor = new Proffessor
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            Gender = Genders.Man,
            EnrollmentDate = DateTime.UtcNow,
            DateOfBirth = DateTime.UtcNow
        };
        var offering = new DisciplineOffering
        {
            Id = Guid.NewGuid(),
            DisciplineId = discipline.Id,
            Discipline = discipline,
            SemesterId = semester.Id,
            Semester = semester,
            StudentGroupId = group.Id,
            StudentGroup = group,
            ProffessorId = professor.Id,
            Proffessor = professor
        };

        db.AcademicYears.Add(year);
        db.Semesters.Add(semester);
        db.StudentGroups.Add(group);
        db.Disciplines.Add(discipline);
        db.Proffessor.Add(professor);
        db.DisciplineOfferings.Add(offering);

        db.GradeSheets.Add(new GradeSheet
        {
            Id = sheetId,
            DisciplineOfferingId = offering.Id,
            DisciplineOffering = offering,
            CreatedAt = DateTime.UtcNow,
            Status = SheetStatus.Closed
        });
        await db.SaveChangesAsync();

        var service = new FinalGradeService(db);
        var action = async () => await service.RecalculateAsync(sheetId);

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task RecalculateAsync_ShouldCalculateExamMarks()
    {
        await using var db = TestDbFactory.CreateContext();

        var student1 = new Student { Id = Guid.NewGuid(), FirstName = "А", LastName = "Б", Gender = Genders.Man, EnrollmentDate = DateTime.UtcNow, DateOfBirth = DateTime.UtcNow };
        var student2 = new Student { Id = Guid.NewGuid(), FirstName = "В", LastName = "Г", Gender = Genders.Woman, EnrollmentDate = DateTime.UtcNow, DateOfBirth = DateTime.UtcNow };

        var group = new StudentGroup { Id = Guid.NewGuid(), Name = "ИС-221", StudyYear = 2, AcademicYearId = Guid.NewGuid(), Students = new List<Student> { student1, student2 } };
        var discipline = new Discipline { Id = Guid.NewGuid(), Name = "БД", Hours = 72, ControlType = ControlType.Exam };
        var offering = new DisciplineOffering
        {
            Id = Guid.NewGuid(),
            DisciplineId = discipline.Id,
            Discipline = discipline,
            SemesterId = Guid.NewGuid(),
            StudentGroupId = group.Id,
            StudentGroup = group,
            ProffessorId = Guid.NewGuid()
        };
        var sheet = new GradeSheet { Id = Guid.NewGuid(), DisciplineOfferingId = offering.Id, DisciplineOffering = offering, CreatedAt = DateTime.UtcNow, Status = SheetStatus.Open };

        db.GradeSheets.Add(sheet);
        db.Grades.AddRange(
            new Grade { StudentId = student1.Id, AssignmentId = 1, Score = 90, DisciplineOfferingId = offering.Id },
            new Grade { StudentId = student1.Id, AssignmentId = 2, Score = 80, DisciplineOfferingId = offering.Id },
            new Grade { StudentId = student2.Id, AssignmentId = 3, Score = 60, DisciplineOfferingId = offering.Id }
        );

        await db.SaveChangesAsync();

        var service = new FinalGradeService(db);
        var finals = await service.RecalculateAsync(sheet.Id);

        finals.Should().HaveCount(2);
        finals.Single(f => f.StudentId == student1.Id).FinalMark.Should().Be("5");
        finals.Single(f => f.StudentId == student2.Id).FinalMark.Should().Be("3");
    }

    [Fact]
    public async Task UpsertManualAsync_ShouldCreateAndUpdate()
    {
        await using var db = TestDbFactory.CreateContext();
        var sheetId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        db.GradeSheets.Add(new GradeSheet
        {
            Id = sheetId,
            DisciplineOfferingId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = SheetStatus.Open
        });
        await db.SaveChangesAsync();

        var service = new FinalGradeService(db);

        var created = await service.UpsertManualAsync(sheetId, studentId, 88m, "4");
        var updated = await service.UpsertManualAsync(sheetId, studentId, 92m, "5");

        created.Id.Should().Be(updated.Id);
        updated.FinalScore.Should().Be(92m);
        updated.FinalMark.Should().Be("5");
    }
}
