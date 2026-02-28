using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.Services;
using WebStudents.UnitTests.Support;
using Xunit;

namespace WebStudents.UnitTests.Services;

public class AssignmentServiceTests
{
    [Fact]
    public async Task AddAsync_ShouldThrow_WhenOfferingIdMissing()
    {
        await using var db = TestDbFactory.CreateContext();
        var service = new AssignmentService(db);

        var action = async () => await service.AddAsync(new Assignment { Title = "A", DueDate = DateTime.UtcNow });

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenOfferingNotFound()
    {
        await using var db = TestDbFactory.CreateContext();
        var service = new AssignmentService(db);

        var action = async () => await service.AddAsync(new Assignment
        {
            Title = "A",
            DueDate = DateTime.UtcNow,
            DisciplineOfferingId = Guid.NewGuid()
        });

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task AddAsync_ShouldPersist_WhenOfferingExists()
    {
        await using var db = TestDbFactory.CreateContext();
        var offering = new DisciplineOffering
        {
            Id = Guid.NewGuid(),
            DisciplineId = Guid.NewGuid(),
            SemesterId = Guid.NewGuid(),
            StudentGroupId = Guid.NewGuid(),
            ProffessorId = Guid.NewGuid()
        };
        db.DisciplineOfferings.Add(offering);
        await db.SaveChangesAsync();

        var service = new AssignmentService(db);
        var assignment = new Assignment { Title = "A", DueDate = DateTime.UtcNow, DisciplineOfferingId = offering.Id };

        var result = await service.AddAsync(assignment);

        result.DisciplineOfferingId.Should().Be(offering.Id);
        db.Assignments.Should().ContainSingle();
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByOffering()
    {
        await using var db = TestDbFactory.CreateContext();
        var offering1 = Guid.NewGuid();
        var offering2 = Guid.NewGuid();

        db.Assignments.AddRange(
            new Assignment { Title = "A1", DueDate = DateTime.UtcNow.AddDays(1), DisciplineOfferingId = offering1 },
            new Assignment { Title = "A2", DueDate = DateTime.UtcNow.AddDays(2), DisciplineOfferingId = offering2 }
        );
        await db.SaveChangesAsync();

        var service = new AssignmentService(db);

        var result = await service.GetAllAsync(offering1);

        result.Should().HaveCount(1);
        result[0].DisciplineOfferingId.Should().Be(offering1);
    }
}
