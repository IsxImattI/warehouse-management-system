using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;
using WMS.Api.DTOs;
using WMS.Api.Services;

namespace WMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuthService _auth;

    public UsersController(AppDbContext db, AuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    // GET api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .Select(u => new { u.Id, u.Username, u.Role, u.CreatedAt })
            .ToListAsync();
        return Ok(users);
    }

    // POST api/users
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await _auth.RegisterAsync(dto.Username, dto.Password, dto.Role);
            return Ok(new { user.Id, user.Username, user.Role });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}