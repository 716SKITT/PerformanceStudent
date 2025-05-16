using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Services;
using StudentsPerformance.Models;
using WebStudents.Dtos;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradeController : ControllerBase
{
    private readonly GradeService _gradeService;

    public GradeController(GradeService gradeService)
    {
        _gradeService = gradeService;
    }

    [HttpGet("student/{studentId}")]
    public IActionResult GetByStudent(Guid studentId)
    {
        var grades = _gradeService.GetGradesByStudent(studentId);
        return Ok(grades);
    }

    [HttpPost]
    public IActionResult Add([FromBody] GradeDto dto)
    {
        var grade = new Grade
        {
            StudentId = dto.StudentId,
            AssignmentId = dto.AssignmentId,
            Score = dto.Score
        };

        _gradeService.AddGrade(grade);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Grade updated)
    {
        updated.Id = id;
        _gradeService.UpdateGrade(updated);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _gradeService.DeleteGrade(id);
        return Ok();
    }
}
