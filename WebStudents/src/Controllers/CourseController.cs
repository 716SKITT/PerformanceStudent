using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.Dtos;
using WebStudents.src.Auth;
using WebStudents.src.Services;

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

    [HttpGet("{id:int}")]
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
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        _courseService.AddCourse(course);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        _courseService.DeleteCourse(id);
        return Ok();
    }
}
