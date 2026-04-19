using Microsoft.AspNetCore.Mvc;
using WMS.Api.DTOs;
using WMS.Api.Services;

namespace WMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthDto dto)
    {
        try
        {
            var user = await _auth.RegisterAsync(dto.Username, dto.Password);
            return Ok(new { user.Id, user.Username, user.Role });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto dto)
    {
        try
        {
            var token = await _auth.LoginAsync(dto.Username, dto.Password);
            return Ok(new { token });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}