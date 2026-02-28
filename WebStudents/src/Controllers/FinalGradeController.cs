using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Auth;
using WebStudents.src.Common;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinalGradeController : ControllerBase
{
    private readonly FinalGradeService _service;
    private readonly GradeSheetService _sheetService;
    private readonly AccessPolicyService _accessPolicy;

    public FinalGradeController(
        FinalGradeService service,
        GradeSheetService sheetService,
        AccessPolicyService accessPolicy)
    {
        _service = service;
        _sheetService = sheetService;
        _accessPolicy = accessPolicy;
    }

    [HttpGet("sheet/{sheetId:guid}")]
    public async Task<IActionResult> GetBySheet(Guid sheetId)
    {
        var user = this.GetUserContext();
        var items = await _service.GetBySheetAsync(sheetId);

        if (user.IsInRole("Student") && user.LinkedPersonId.HasValue)
        {
            items = items.Where(x => x.StudentId == user.LinkedPersonId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpPost("{sheetId:guid}/recalculate")]
    public async Task<IActionResult> Recalculate(Guid sheetId)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        try
        {
            var sheet = await _sheetService.GetByIdAsync(sheetId);
            if (sheet == null)
            {
                return NotFound();
            }

            var user = this.GetUserContext();
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(sheet.DisciplineOfferingId, user.LinkedPersonId);

            var result = await _service.RecalculateAsync(sheetId);
            return Ok(result);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("{sheetId:guid}/{studentId:guid}")]
    public async Task<IActionResult> SetManual(Guid sheetId, Guid studentId, [FromBody] ManualFinalGradeRequest request)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        try
        {
            var sheet = await _sheetService.GetByIdAsync(sheetId);
            if (sheet == null)
            {
                return NotFound();
            }

            var user = this.GetUserContext();
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(sheet.DisciplineOfferingId, user.LinkedPersonId);

            var result = await _service.UpsertManualAsync(sheetId, studentId, request.FinalScore, request.FinalMark);
            return Ok(result);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}

public class ManualFinalGradeRequest
{
    public decimal? FinalScore { get; set; }
    public string? FinalMark { get; set; }
}
