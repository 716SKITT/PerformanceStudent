using Microsoft.AspNetCore.Mvc;
using StudentsPerformance.Models;
using WebStudents.Dtos;
using WebStudents.src.Auth;
using WebStudents.src.Common;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradeController : ControllerBase
{
    private readonly GradeService _gradeService;
    private readonly AccessPolicyService _accessPolicy;

    public GradeController(GradeService gradeService, AccessPolicyService accessPolicy)
    {
        _gradeService = gradeService;
        _accessPolicy = accessPolicy;
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentId)
    {
        var user = this.GetUserContext();
        if (user.IsInRole("Student") && user.LinkedPersonId != studentId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Можно смотреть только свои оценки" });
        }

        var grades = await _gradeService.GetGradesByStudentAsync(studentId);
        return Ok(grades);
    }

    [HttpGet("offering/{offeringId:guid}")]
    public async Task<IActionResult> GetByOffering(Guid offeringId)
    {
        var user = this.GetUserContext();
        if (user.IsInRole("Professor"))
        {
            try
            {
                await _accessPolicy.EnsureProfessorOwnsOfferingAsync(offeringId, user.LinkedPersonId);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
        }

        return Ok(await _gradeService.GetByOfferingAsync(offeringId));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] GradeDto dto)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        var grade = new Grade
        {
            StudentId = dto.StudentId,
            AssignmentId = dto.AssignmentId,
            Score = dto.Score,
            DisciplineOfferingId = dto.DisciplineOfferingId
        };

        var user = this.GetUserContext();

        try
        {
            var offeringId = dto.DisciplineOfferingId ?? await _accessPolicy.ResolveAssignmentOfferingAsync(dto.AssignmentId);

            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(offeringId, user.LinkedPersonId);
            await _accessPolicy.EnsureOfferingIsOpenAsync(offeringId);
            grade.DisciplineOfferingId = offeringId;
            var created = await _gradeService.AddGradeAsync(grade);
            return Ok(created);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Grade updated)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        updated.Id = id;

        var user = this.GetUserContext();

        try
        {
            var offeringId = await _accessPolicy.ResolveGradeOfferingAsync(id);
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(offeringId, user.LinkedPersonId);
            await _accessPolicy.EnsureOfferingIsOpenAsync(offeringId);

            var changed = await _gradeService.UpdateGradeAsync(updated);
            return changed ? Ok() : NotFound();
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
            var offeringId = await _accessPolicy.ResolveGradeOfferingAsync(id);
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(offeringId, user.LinkedPersonId);
            await _accessPolicy.EnsureOfferingIsOpenAsync(offeringId);

            var deleted = await _gradeService.DeleteGradeAsync(id);
            return deleted ? Ok() : NotFound();
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
