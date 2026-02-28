using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.Services;
using WebStudents.UnitTests.Support;
using Xunit;

namespace WebStudents.UnitTests.Services;

public class GradeSheetServiceTests
{
    [Fact]
    public async Task CreateForOfferingAsync_ShouldCreateOpenSheet()
    {
        await using var db = TestDbFactory.CreateContext();
        var offeringId = Guid.NewGuid();

        db.DisciplineOfferings.Add(new DisciplineOffering
        {
            Id = offeringId,
            DisciplineId = Guid.NewGuid(),
            SemesterId = Guid.NewGuid(),
            StudentGroupId = Guid.NewGuid(),
            ProffessorId = Guid.NewGuid()
        });
        await db.SaveChangesAsync();

        var service = new GradeSheetService(db, new AccessPolicyService(db));

        var sheet = await service.CreateForOfferingAsync(offeringId);

        sheet.DisciplineOfferingId.Should().Be(offeringId);
        sheet.Status.Should().Be(SheetStatus.Open);
    }

    [Fact]
    public async Task CreateForOfferingAsync_ShouldThrow_WhenOfferingMissing()
    {
        await using var db = TestDbFactory.CreateContext();
        var service = new GradeSheetService(db, new AccessPolicyService(db));

        var action = async () => await service.CreateForOfferingAsync(Guid.NewGuid());

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task CloseAsync_ShouldThrow_WhenAlreadyClosed()
    {
        await using var db = TestDbFactory.CreateContext();
        var sheetId = Guid.NewGuid();

        db.GradeSheets.Add(new GradeSheet
        {
            Id = sheetId,
            DisciplineOfferingId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = SheetStatus.Closed
        });
        await db.SaveChangesAsync();

        var service = new GradeSheetService(db, new AccessPolicyService(db));

        var action = async () => await service.CloseAsync(sheetId);

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }
}
