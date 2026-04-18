using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;
using WMS.Api.Models;

namespace WMS.Api.Services;

public record MoveRequest(int ItemId, int? FromLocationId, int? ToLocationId, decimal Quantity, string? Note);

public class MovementService
{
    private readonly AppDbContext _db;

    public MovementService(AppDbContext db) => _db = db;

    public async Task ExecuteAsync(MoveRequest req)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            if (req.FromLocationId.HasValue)
            {
                var from = await _db.Inventory
                    .FirstOrDefaultAsync(i => i.ItemId == req.ItemId && i.LocationId == req.FromLocationId)
                    ?? throw new InvalidOperationException("Artikel ni na izvorni lokaciji.");

                if (from.Quantity < req.Quantity)
                    throw new InvalidOperationException($"Premalo zaloge: {from.Quantity}");

                from.Quantity -= req.Quantity;
                from.UpdatedAt = DateTime.UtcNow;
            }

            if (req.ToLocationId.HasValue)
            {
                var to = await _db.Inventory
                    .FirstOrDefaultAsync(i => i.ItemId == req.ItemId && i.LocationId == req.ToLocationId);

                if (to == null)
                {
                    to = new Inventory { ItemId = req.ItemId, LocationId = req.ToLocationId.Value, Quantity = 0 };
                    _db.Inventory.Add(to);
                }

                to.Quantity += req.Quantity;
                to.UpdatedAt = DateTime.UtcNow;
            }

            _db.MovementLogs.Add(new MovementLog
            {
                ItemId = req.ItemId,
                FromLocationId = req.FromLocationId,
                ToLocationId = req.ToLocationId,
                Quantity = req.Quantity,
                Note = req.Note
            });

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}