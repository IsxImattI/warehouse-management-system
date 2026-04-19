using Microsoft.EntityFrameworkCore;
using WMS.Api.Data;
using WMS.Api.DTOs;
using WMS.Api.Models;

namespace WMS.Api.Services;

public class MovementService
{
    private readonly AppDbContext _db;

    public MovementService(AppDbContext db) => _db = db;

    public async Task ExecuteAsync(MoveRequestDto req, int? userId = null)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            if (req.FromLocationId.HasValue)
            {
                var from = await _db.Inventory
                    .FirstOrDefaultAsync(i => i.ItemId == req.ItemId && i.LocationId == req.FromLocationId)
                    ?? throw new InvalidOperationException("Item not found at source location.");

                if (from.Quantity < req.Quantity)
                    throw new InvalidOperationException($"Insufficient stock: {from.Quantity}");

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
                Note = req.Note,
                CreatedById = userId
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