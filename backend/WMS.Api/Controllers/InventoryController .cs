using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;

namespace WMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public InventoryController(AppDbContext db) => _db = db;

    // GET api/inventory
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inventory = await _db.Inventory
            .Include(i => i.Item)
            .Include(i => i.Location)
            .Select(i => new
            {
                i.Id,
                Item = new { i.Item.Id, i.Item.SKU, i.Item.Name },
                Location = new { i.Location.Id, i.Location.Code, i.Location.Name },
                i.Quantity,
                i.UpdatedAt
            })
            .ToListAsync();

        return Ok(inventory);
    }

    // GET api/inventory/location/5
    [HttpGet("location/{locationId}")]
    public async Task<IActionResult> GetByLocation(int locationId)
    {
        var inventory = await _db.Inventory
            .Include(i => i.Item)
            .Include(i => i.Location)
            .Where(i => i.LocationId == locationId)
            .Select(i => new
            {
                i.Id,
                Item = new { i.Item.Id, i.Item.SKU, i.Item.Name },
                i.Quantity,
                i.UpdatedAt
            })
            .ToListAsync();

        return Ok(inventory);
    }

    // GET api/inventory/item/5
    [HttpGet("item/{itemId}")]
    public async Task<IActionResult> GetByItem(int itemId)
    {
        var inventory = await _db.Inventory
            .Include(i => i.Item)
            .Include(i => i.Location)
            .Where(i => i.ItemId == itemId)
            .Select(i => new
            {
                i.Id,
                Location = new { i.Location.Id, i.Location.Code, i.Location.Name },
                i.Quantity,
                i.UpdatedAt
            })
            .ToListAsync();

        return Ok(inventory);
    }
}