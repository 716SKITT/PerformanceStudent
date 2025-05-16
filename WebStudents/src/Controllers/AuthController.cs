using Microsoft.AspNetCore.Mvc;
using WebStudents.src.Services;
using StudentsPerformance.Models;
using WebStudents.Dtos;

namespace WebStudents.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _authService.Authenticate(dto.Username, dto.Password);
        if (user == null) return Unauthorized("Неверные данные");

        return Ok(new { user.Username, user.Role, user.LinkedPersonId });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        var user = new UserAccount
        {
            Username = dto.Username,
            PasswordHash = dto.Password,
            Role = dto.Role,
            LinkedPersonId = dto.LinkedPersonId
        };

        _authService.Register(user);
        return Ok();
    }
}
