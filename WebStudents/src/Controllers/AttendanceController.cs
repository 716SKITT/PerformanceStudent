using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.src.Auth;
using WebStudents.src.Common;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _attendanceService;
    private readonly AccessPolicyService _accessPolicy;

    public AttendanceController(AttendanceService attendanceService, AccessPolicyService accessPolicy)
    {
        _attendanceService = attendanceService;
        _accessPolicy = accessPolicy;
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentId)
    {
        var user = this.GetUserContext();
        if (user.IsInRole("Student") && user.LinkedPersonId != studentId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Можно смотреть только свою посещаемость" });
        }

        var records = await _attendanceService.GetByStudentAsync(studentId);
        return Ok(records);
    }

    [HttpGet("offering/{offeringId:guid}")]
    public async Task<IActionResult> GetByOffering(Guid offeringId)
    {
        var user = this.GetUserContext();
        if (user.IsInRole("Professor"))
        {
            try
            {
                await _accessPolicy.EnsureProfessorOwnsOfferingAsync(offeringId, user.LinkedPersonId);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
        }

        return Ok(await _attendanceService.GetByOfferingAsync(offeringId));
    }

    [HttpPost]
    public async Task<IActionResult> Mark([FromBody] Attendance attendance)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        if (!attendance.DisciplineOfferingId.HasValue)
        {
            return BadRequest(new { message = "DisciplineOfferingId обязателен" });
        }

        var user = this.GetUserContext();

        try
        {
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(attendance.DisciplineOfferingId.Value, user.LinkedPersonId);
            await _accessPolicy.EnsureOfferingIsOpenAsync(attendance.DisciplineOfferingId.Value);

            var result = await _attendanceService.MarkAttendanceAsync(attendance);
            return Ok(result);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
