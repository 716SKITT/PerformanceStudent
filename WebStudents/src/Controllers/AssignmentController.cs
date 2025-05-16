using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Services;
using StudentsPerformance.Models;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly AssignmentService _assignmentService;

    public AssignmentController(AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = _assignmentService.GetAll();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var assignment = _assignmentService.GetById(id);
        if (assignment == null) return NotFound();

        return Ok(assignment);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Assignment assignment)
    {
        _assignmentService.Add(assignment);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _assignmentService.Delete(id);
        return Ok();
    }
}
