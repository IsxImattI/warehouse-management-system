using Microsoft.AspNetCore.Mvc;
using WMS.Api.DTOs;
using WMS.Api.Services;

namespace WMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovementController : ControllerBase
{
    private readonly MovementService _svc;

    public MovementController(MovementService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Move([FromBody] MoveRequestDto req)
    {
        try
        {
            await _svc.ExecuteAsync(req);
            return Ok(new { message = "Movement successful." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}