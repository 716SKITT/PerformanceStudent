using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.src.Auth;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentGroupController : ControllerBase
{
    private readonly StudentGroupService _service;

    public StudentGroupController(StudentGroupService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentGroup model)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        var created = await _service.CreateAsync(model);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] StudentGroup model)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        var updated = await _service.UpdateAsync(id, model);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        var deleted = await _service.DeleteAsync(id);
        return deleted ? Ok() : NotFound();
    }
}
