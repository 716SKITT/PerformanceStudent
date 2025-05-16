using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Services;
using StudentsPerformance.Models;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _attendanceService;

    public AttendanceController(AttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpGet("student/{studentId}")]
    public IActionResult GetByStudent(Guid studentId)
    {
        var records = _attendanceService.GetByStudent(studentId);
        return Ok(records);
    }

    [HttpPost]
    public IActionResult Mark([FromBody] Attendance attendance)
    {
        _attendanceService.MarkAttendance(attendance);
        return Ok();
    }
}
