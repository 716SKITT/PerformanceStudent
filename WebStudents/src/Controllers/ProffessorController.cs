using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Services;
using StudentsPerformance.Models;

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

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var professor = _proffessorService.GetById(id);
        if (professor == null) return NotFound();
        return Ok(professor);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Proffessor proffessor)
    {
        _proffessorService.Add(proffessor);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] Proffessor updated)
    {
        var existing = _proffessorService.GetById(id);
        if (existing == null) return NotFound();

        updated.Id = id;
        _proffessorService.Update(updated);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _proffessorService.Delete(id);
        return Ok();
    }
}
