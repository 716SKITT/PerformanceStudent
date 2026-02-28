using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.Services;
using WebStudents.UnitTests.Support;
using Xunit;

namespace WebStudents.UnitTests.Services;

public class GradeServiceTests
{
    [Fact]
    public async Task AddGradeAsync_ShouldThrow_WhenAssignmentNotFound()
    {
        await using var db = TestDbFactory.CreateContext();
        var service = new GradeService(db);

        var action = async () => await service.AddGradeAsync(new Grade
        {
            StudentId = Guid.NewGuid(),
            AssignmentId = 999,
            Score = 90
        });

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task AddGradeAsync_ShouldUseAssignmentOffering_WhenMissingInGrade()
    {
        await using var db = TestDbFactory.CreateContext();
        var offeringId = Guid.NewGuid();

        db.Assignments.Add(new Assignment
        {
            Id = 1,
            Title = "Lab",
            DueDate = DateTime.UtcNow,
            DisciplineOfferingId = offeringId
        });
        await db.SaveChangesAsync();

        var service = new GradeService(db);

        var grade = await service.AddGradeAsync(new Grade
        {
            StudentId = Guid.NewGuid(),
            AssignmentId = 1,
            Score = 75
        });

        grade.DisciplineOfferingId.Should().Be(offeringId);
    }

    [Fact]
    public async Task AddGradeAsync_ShouldThrow_WhenNoOfferingInBothGradeAndAssignment()
    {
        await using var db = TestDbFactory.CreateContext();

        db.Assignments.Add(new Assignment
        {
            Id = 2,
            Title = "Lab",
            DueDate = DateTime.UtcNow,
            DisciplineOfferingId = null
        });
        await db.SaveChangesAsync();

        var service = new GradeService(db);

        var action = async () => await service.AddGradeAsync(new Grade
        {
            StudentId = Guid.NewGuid(),
            AssignmentId = 2,
            Score = 80
        });

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
