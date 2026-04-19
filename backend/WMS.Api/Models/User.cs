namespace WMS.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "operator"; // operator | admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}