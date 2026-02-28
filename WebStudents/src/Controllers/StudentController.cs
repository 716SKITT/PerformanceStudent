using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.src.Auth;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly StudentService _studentService;

    public StudentController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var forbid = this.RequireRoles("Admin", "Professor");
        if (forbid != null) return forbid;

        var students = _studentService.GetAllStudents();
        return Ok(students);
    }

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var user = this.GetUserContext();
        if (user.IsInRole("Student") && user.LinkedPersonId != id)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Можно смотреть только свой профиль" });
        }

        var student = _studentService.GetStudentById(id);
        if (student == null) return NotFound();

        return Ok(student);
    }

    [HttpGet("me/summary")]
    public async Task<IActionResult> GetMySummary()
    {
        var forbid = this.RequireRoles("Student");
        if (forbid != null) return forbid;

        var user = this.GetUserContext();
        if (!user.LinkedPersonId.HasValue)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "LinkedPersonId обязателен" });
        }

        var summary = await _studentService.GetStudentSummaryAsync(user.LinkedPersonId.Value);
        return summary == null ? NotFound() : Ok(summary);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Student student)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        _studentService.AddStudent(student);
        return Ok();
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] Student updated)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        var existing = _studentService.GetStudentById(id);
        if (existing == null) return NotFound();

        updated.Id = id;
        _studentService.UpdateStudent(updated);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        _studentService.DeleteStudent(id);
        return Ok();
    }
}
