using Microsoft.EntityFrameworkCore;
using WMS.Api.Models;

namespace WMS.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Inventory> Inventory => Set<Inventory>();
    public DbSet<MovementLog> MovementLogs => Set<MovementLog>();

    protected override void OnModelCreating(ModelBuilder mb)
{
    mb.Entity<Inventory>()
        .HasIndex(i => new { i.ItemId, i.LocationId })
        .IsUnique();

    mb.Entity<Inventory>()
        .Property(i => i.Quantity)
        .HasPrecision(18, 3);

    mb.Entity<MovementLog>()
        .Property(m => m.Quantity)
        .HasPrecision(18, 3);
}
}