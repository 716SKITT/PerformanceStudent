using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.src.Auth;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DisciplineOfferingController : ControllerBase
{
    private readonly DisciplineOfferingService _service;

    public DisciplineOfferingController(DisciplineOfferingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = this.GetUserContext();

        if (user.IsInRole("Professor") && user.LinkedPersonId.HasValue)
        {
            return Ok(await _service.GetMyAsync(user.LinkedPersonId.Value));
        }

        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        var user = this.GetUserContext();
        if (!user.LinkedPersonId.HasValue)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "LinkedPersonId обязателен" });
        }

        return Ok(await _service.GetMyAsync(user.LinkedPersonId.Value));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DisciplineOffering model)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        var created = await _service.CreateAsync(model);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DisciplineOffering model)
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
