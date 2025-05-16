using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Services;
using StudentsPerformance.Models;
using WebStudents.Dtos;
namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly CourseService _courseService;

    public CourseController(CourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var courses = _courseService.GetAllCourses();
        return Ok(courses);
    }

    [HttpGet("{id}")]
public IActionResult Get(int id)
{
    var course = _courseService.GetCourseById(id);
    if (course == null) return NotFound();

    var dto = new CourseDto
    {
        Id = course.Id,
        Name = course.Name,
        Students = course.Students
            .Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName
            }).ToList()
    };

    return Ok(dto);
}


    [HttpPost]
    public IActionResult Add([FromBody] Course course)
    {
        _courseService.AddCourse(course);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _courseService.DeleteCourse(id);
        return Ok();
    }
}
