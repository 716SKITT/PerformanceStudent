using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudentsPerformance.Models;
using WebStudents.src.Common;
using WebStudents.src.Services;
using WebStudents.UnitTests.Support;
using Xunit;

namespace WebStudents.UnitTests.Services;

public class AttendanceServiceTests
{
    [Fact]
    public async Task MarkAttendanceAsync_ShouldThrow_WhenOfferingMissing()
    {
        await using var db = TestDbFactory.CreateContext();
        var service = new AttendanceService(db);

        var action = async () => await service.MarkAttendanceAsync(new Attendance
        {
            StudentId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            IsPresent = true
        });

        var ex = await Assert.ThrowsAsync<ApiException>(action);
        ex.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task MarkAttendanceAsync_ShouldCreateOrUpdateDailyMark()
    {
        await using var db = TestDbFactory.CreateContext();
        var offeringId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        db.DisciplineOfferings.Add(new DisciplineOffering
        {
            Id = offeringId,
            DisciplineId = Guid.NewGuid(),
            SemesterId = Guid.NewGuid(),
            StudentGroupId = Guid.NewGuid(),
            ProffessorId = Guid.NewGuid()
        });
        await db.SaveChangesAsync();

        var service = new AttendanceService(db);

        await service.MarkAttendanceAsync(new Attendance
        {
            StudentId = studentId,
            Date = new DateTime(2026, 1, 15),
            IsPresent = false,
            DisciplineOfferingId = offeringId
        });

        await service.MarkAttendanceAsync(new Attendance
        {
            StudentId = studentId,
            Date = new DateTime(2026, 1, 15),
            IsPresent = true,
            DisciplineOfferingId = offeringId
        });

        var all = await db.Attendances.ToListAsync();
        all.Should().HaveCount(1);
        all[0].IsPresent.Should().BeTrue();
    }
}
