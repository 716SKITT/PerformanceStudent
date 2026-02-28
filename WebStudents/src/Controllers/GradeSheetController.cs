using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Auth;
using WebStudents.src.Common;
using WebStudents.src.Services;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradeSheetController : ControllerBase
{
    private readonly GradeSheetService _service;
    private readonly AccessPolicyService _accessPolicy;

    public GradeSheetController(GradeSheetService service, AccessPolicyService accessPolicy)
    {
        _service = service;
        _accessPolicy = accessPolicy;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("offering/{offeringId:guid}")]
    public async Task<IActionResult> GetByOffering(Guid offeringId)
    {
        return Ok(await _service.GetByOfferingAsync(offeringId));
    }

    [HttpPost("{offeringId:guid}/create")]
    public async Task<IActionResult> Create(Guid offeringId)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        var user = this.GetUserContext();

        try
        {
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(offeringId, user.LinkedPersonId);
            var created = await _service.CreateForOfferingAsync(offeringId);
            return Ok(created);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPost("{sheetId:guid}/close")]
    public async Task<IActionResult> Close(Guid sheetId)
    {
        var forbid = this.RequireRoles("Professor");
        if (forbid != null) return forbid;

        try
        {
            var sheet = await _service.GetByIdAsync(sheetId);
            if (sheet == null)
            {
                return NotFound();
            }

            var user = this.GetUserContext();
            await _accessPolicy.EnsureProfessorOwnsOfferingAsync(sheet.DisciplineOfferingId, user.LinkedPersonId);

            var closed = await _service.CloseAsync(sheetId);
            return Ok(closed);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
