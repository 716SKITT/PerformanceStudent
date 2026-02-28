using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.src.Auth;
using WebStudents.src.Common;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly AssignmentService _assignmentService;
    private readonly AccessPolicyService _accessPolicy;

    public AssignmentController(AssignmentService assignmentService, AccessPolicyService accessPolicy)
    {
        _assignmentService = assignmentService;
        _accessPolicy = accessPolicy;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? offeringId)
    {
        var list = await _assignmentService.GetAllAsync(offeringId);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var assignment = await _assignmentService.GetByIdAsync(id);
        if (assignment == null) return NotFound();

        return Ok(assignment);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Assignment assignment)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        if (!assignment.DisciplineOfferingId.HasValue)
        {
            return BadRequest(new { message = "DisciplineOfferingId обязателен" });
        }

        var user = this.GetUserContext();

        try
        {
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(assignment.DisciplineOfferingId.Value, user.LinkedPersonId);
            await _accessPolicy.EnsureOfferingIsOpenAsync(assignment.DisciplineOfferingId.Value);
            var created = await _assignmentService.AddAsync(assignment);
            return Ok(created);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        var user = this.GetUserContext();

        try
        {
            var offeringId = await _accessPolicy.ResolveAssignmentOfferingAsync(id);
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(offeringId, user.LinkedPersonId);
            await _accessPolicy.EnsureOfferingIsOpenAsync(offeringId);

            var deleted = await _assignmentService.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
