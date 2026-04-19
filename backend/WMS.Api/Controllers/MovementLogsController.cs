using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;

namespace WMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovementLogsController : ControllerBase
{
    private readonly AppDbContext _db;

    public MovementLogsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _db.MovementLogs
            .Include(l => l.Item)
            .Include(l => l.FromLocation)
            .Include(l => l.ToLocation)
            .Include(l => l.CreatedBy)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new
            {
                l.Id,
                Item = new { l.Item.Id, l.Item.SKU, l.Item.Name },
                FromLocation = l.FromLocation == null ? null : new { l.FromLocation.Id, l.FromLocation.Code, l.FromLocation.Name },
                ToLocation = l.ToLocation == null ? null : new { l.ToLocation.Id, l.ToLocation.Code, l.ToLocation.Name },
                l.Quantity,
                l.Note,
                CreatedBy = l.CreatedBy == null ? null : new { l.CreatedBy.Id, l.CreatedBy.Username },
                l.CreatedAt
            })
            .ToListAsync();

        return Ok(logs);
    }
}