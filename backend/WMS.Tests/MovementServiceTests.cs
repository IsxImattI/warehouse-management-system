using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;
using WMS.Api.Models;
using WMS.Api.Services;
using WMS.Api.DTOs;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.InMemory.Internal;

namespace WMS.Tests;

public class MovementServiceTests
{
    private AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(Item item, Location locA, Location locB)> SeedBasicData(AppDbContext db)
    {
        var item = new Item { SKU = "TEST-001", Name = "Test Item" };
        var locA = new Location { Code = "LOC-A", Name = "Location A" };
        var locB = new Location { Code = "LOC-B", Name = "Location B" };

        db.Items.Add(item);
        db.Locations.AddRange(locA, locB);
        await db.SaveChangesAsync();

        return (item, locA, locB);
    }

    // Incoming stock (from = null)
    [Fact]
    public async Task Move_IncomingStock_CreatesInventoryRecord()
    {
        var db = CreateInMemoryDb();
        var svc = new MovementService(db);
        var (item, locA, _) = await SeedBasicData(db);

        await svc.ExecuteAsync(new MoveRequestDto(item.Id, null, locA.Id, 10, "Initial stock"));

        var inv = await db.Inventory.FirstOrDefaultAsync(i => i.ItemId == item.Id && i.LocationId == locA.Id);
        Assert.NotNull(inv);
        Assert.Equal(10, inv.Quantity);
    }

    // Move between locations
    [Fact]
    public async Task Move_BetweenLocations_UpdatesBothInventories()
    {
        var db = CreateInMemoryDb();
        var svc = new MovementService(db);
        var (item, locA, locB) = await SeedBasicData(db);

        // First incoming
        await svc.ExecuteAsync(new MoveRequestDto(item.Id, null, locA.Id, 10, null));

        // Move 4 units to locB
        await svc.ExecuteAsync(new MoveRequestDto(item.Id, locA.Id, locB.Id, 4, null));

        var invA = await db.Inventory.FirstAsync(i => i.ItemId == item.Id && i.LocationId == locA.Id);
        var invB = await db.Inventory.FirstAsync(i => i.ItemId == item.Id && i.LocationId == locB.Id);

        Assert.Equal(6, invA.Quantity);
        Assert.Equal(4, invB.Quantity);
    }

    // Movement is logged
    [Fact]
    public async Task Move_CreatesMovementLog()
    {
        var db = CreateInMemoryDb();
        var svc = new MovementService(db);
        var (item, locA, _) = await SeedBasicData(db);

        await svc.ExecuteAsync(new MoveRequestDto(item.Id, null, locA.Id, 5, "Test log"));

        var log = await db.MovementLogs.FirstOrDefaultAsync();
        Assert.NotNull(log);
        Assert.Equal(5, log.Quantity);
        Assert.Equal("Test log", log.Note);
    }

    // Insufficient stock throws
    [Fact]
    public async Task Move_InsufficientStock_ThrowsException()
    {
        var db = CreateInMemoryDb();
        var svc = new MovementService(db);
        var (item, locA, locB) = await SeedBasicData(db);

        await svc.ExecuteAsync(new MoveRequestDto(item.Id, null, locA.Id, 5, null));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.ExecuteAsync(new MoveRequestDto(item.Id, locA.Id, locB.Id, 99, null)));
    }

    // Item not at source location throws
    [Fact]
    public async Task Move_ItemNotAtSourceLocation_ThrowsException()
    {
        var db = CreateInMemoryDb();
        var svc = new MovementService(db);
        var (item, locA, locB) = await SeedBasicData(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.ExecuteAsync(new MoveRequestDto(item.Id, locA.Id, locB.Id, 5, null)));
    }
}