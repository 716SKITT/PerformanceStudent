using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.src.Auth;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProffessorController : ControllerBase
{
    private readonly ProffessorService _proffessorService;

    public ProffessorController(ProffessorService proffessorService)
    {
        _proffessorService = proffessorService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = _proffessorService.GetAll();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var professor = _proffessorService.GetById(id);
        if (professor == null) return NotFound();
        return Ok(professor);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Proffessor proffessor)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        _proffessorService.Add(proffessor);
        return Ok();
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, [FromBody] Proffessor updated)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        var existing = _proffessorService.GetById(id);
        if (existing == null) return NotFound();

        updated.Id = id;
        _proffessorService.Update(updated);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var forbid = this.RequireRoles("Admin");
        if (forbid != null) return forbid;

        _proffessorService.Delete(id);
        return Ok();
    }
}
