using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;
using WMS.Api.DTOs;
using WMS.Api.Models;

namespace WMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public LocationsController(AppDbContext db) => _db = db;

    // GET api/locations
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var locations = await _db.Locations.ToListAsync();
        return Ok(locations);
    }

    // GET api/locations/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var location = await _db.Locations.FindAsync(id);
        return location is null ? NotFound() : Ok(location);
    }

    // POST api/locations
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LocationDto dto)
    {
        var location = new Location
        {
            Code = dto.Code,
            Name = dto.Name,
            QRCode = dto.QRCode
        };

        _db.Locations.Add(location);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
    }

    // PUT api/locations/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] LocationDto dto)
    {
        var location = await _db.Locations.FindAsync(id);
        if (location is null) return NotFound();

        location.Code = dto.Code;
        location.Name = dto.Name;
        location.QRCode = dto.QRCode;

        await _db.SaveChangesAsync();
        return Ok(location);
    }

    // DELETE api/locations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _db.Locations.FindAsync(id);
        if (location is null) return NotFound();

        _db.Locations.Remove(location);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record LocationDto(string Code, string Name, string? QRCode);