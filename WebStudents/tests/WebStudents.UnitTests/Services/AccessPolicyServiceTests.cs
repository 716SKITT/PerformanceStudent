using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.Services;
using WebStudents.UnitTests.Support;
using Xunit;

namespace WebStudents.UnitTests.Services;

public class AccessPolicyServiceTests
{
    [Fact]
    public async Task EnsureProfessorOwnsOfferingAsync_ShouldThrow_WhenProfessorIdMissing()
    {
        await using var db = TestDbFactory.CreateContext();
        var service = new AccessPolicyService(db);

        var action = async () => await service.EnsureProfessorOwnsOfferingAsync(Guid.NewGuid(), null);

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task EnsureProfessorOwnsOfferingAsync_ShouldThrow_WhenNotOwner()
    {
        await using var db = TestDbFactory.CreateContext();
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        db.DisciplineOfferings.Add(new DisciplineOffering
        {
            Id = Guid.NewGuid(),
            DisciplineId = Guid.NewGuid(),
            SemesterId = Guid.NewGuid(),
            StudentGroupId = Guid.NewGuid(),
            ProffessorId = ownerId
        });
        await db.SaveChangesAsync();

        var service = new AccessPolicyService(db);
        var offeringId = db.DisciplineOfferings.First().Id;

        var action = async () => await service.EnsureProfessorOwnsOfferingAsync(offeringId, otherId);

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task EnsureOfferingIsOpenAsync_ShouldThrow_WhenClosedSheetExists()
    {
        await using var db = TestDbFactory.CreateContext();
        var offeringId = Guid.NewGuid();

        db.GradeSheets.Add(new GradeSheet
        {
            Id = Guid.NewGuid(),
            DisciplineOfferingId = offeringId,
            CreatedAt = DateTime.UtcNow,
            Status = SheetStatus.Closed
        });
        await db.SaveChangesAsync();

        var service = new AccessPolicyService(db);

        var action = async () => await service.EnsureOfferingIsOpenAsync(offeringId);

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task ResolveGradeOfferingAsync_ShouldFallbackToAssignmentOffering()
    {
        await using var db = TestDbFactory.CreateContext();
        var offeringId = Guid.NewGuid();

        db.Assignments.Add(new Assignment { Id = 100, Title = "A", DueDate = DateTime.UtcNow, DisciplineOfferingId = offeringId });
        db.Grades.Add(new Grade { Id = 200, StudentId = Guid.NewGuid(), AssignmentId = 100, Score = 90, DisciplineOfferingId = null });
        await db.SaveChangesAsync();

        var service = new AccessPolicyService(db);

        var resolved = await service.ResolveGradeOfferingAsync(200);

        resolved.Should().Be(offeringId);
    }
}
