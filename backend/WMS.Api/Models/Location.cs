namespace WMS.Api.Models;
public class Location
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? QRCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}