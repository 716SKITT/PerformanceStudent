using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Services;
using StudentsPerformance.Models;

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
        var students = _studentService.GetAllStudents();
        return Ok(students);
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var student = _studentService.GetStudentById(id);
        if (student == null) return NotFound();

        return Ok(student);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Student student)
    {
        _studentService.AddStudent(student);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] Student updated)
    {
        var existing = _studentService.GetStudentById(id);
        if (existing == null) return NotFound();

        updated.Id = id;
        _studentService.UpdateStudent(updated);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _studentService.DeleteStudent(id);
        return Ok();
    }
}
