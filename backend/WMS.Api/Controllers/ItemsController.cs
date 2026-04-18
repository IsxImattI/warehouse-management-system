using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;
using WMS.Api.DTOs;
using WMS.Api.Models;

namespace WMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ItemsController(AppDbContext db) => _db = db;

    // GET api/items
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Items.ToListAsync();
        return Ok(items);
    }

    // GET api/items/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.Items.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    // POST api/items
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ItemDto dto)
    {
        var item = new Item
        {
            SKU = dto.SKU,
            EAN = dto.EAN,
            Name = dto.Name,
            Description = dto.Description
        };

        _db.Items.Add(item);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // PUT api/items/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ItemDto dto)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return NotFound();

        item.SKU = dto.SKU;
        item.EAN = dto.EAN;
        item.Name = dto.Name;
        item.Description = dto.Description;

        await _db.SaveChangesAsync();
        return Ok(item);
    }

    // DELETE api/items/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item is null) return NotFound();

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

// DTO — for now in the same file
public record ItemDto(string SKU, string? EAN, string Name, string? Description);