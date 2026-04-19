using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WMS.Api.Data;
using WMS.Api.Services;

namespace WMS.Tests;

public class AuthServiceTests
{
    private AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AppDbContext(options);
    }

    // Register successful
    [Fact]
    public async Task Register_ValidUser_CreatesUser()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthService(db);

        var user = await svc.RegisterAsync("testuser", "password123");

        Assert.NotNull(user);
        Assert.Equal("testuser", user.Username);
        Assert.Equal("operator", user.Role);
        Assert.NotEqual("password123", user.PasswordHash); // must be hashed
    }

    // Register with admin role
    [Fact]
    public async Task Register_WithAdminRole_CreatesAdminUser()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthService(db);

        var user = await svc.RegisterAsync("adminuser", "password123", "admin");

        Assert.Equal("admin", user.Role);
    }

    // Register duplicate username
    [Fact]
    public async Task Register_DuplicateUsername_ThrowsException()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthService(db);

        await svc.RegisterAsync("testuser", "password123");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.RegisterAsync("testuser", "different123"));
    }

    // Login successful - returns token
    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthService(db);

        // Set JWT env vars for token generation
        Environment.SetEnvironmentVariable("JWT__SECRET", "test-secret-key-that-is-long-enough-32chars");
        Environment.SetEnvironmentVariable("JWT__ISSUER", "wms-api");
        Environment.SetEnvironmentVariable("JWT__AUDIENCE", "wms-client");

        await svc.RegisterAsync("testuser", "password123");
        var token = await svc.LoginAsync("testuser", "password123");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    // Login wrong password
    [Fact]
    public async Task Login_WrongPassword_ThrowsException()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthService(db);

        Environment.SetEnvironmentVariable("JWT__SECRET", "test-secret-key-that-is-long-enough-32chars");
        Environment.SetEnvironmentVariable("JWT__ISSUER", "wms-api");
        Environment.SetEnvironmentVariable("JWT__AUDIENCE", "wms-client");

        await svc.RegisterAsync("testuser", "password123");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.LoginAsync("testuser", "wrongpassword"));
    }

    // Login non-existent user
    [Fact]
    public async Task Login_NonExistentUser_ThrowsException()
    {
        var db = CreateInMemoryDb();
        var svc = new AuthService(db);

        Environment.SetEnvironmentVariable("JWT__SECRET", "test-secret-key-that-is-long-enough-32chars");
        Environment.SetEnvironmentVariable("JWT__ISSUER", "wms-api");
        Environment.SetEnvironmentVariable("JWT__AUDIENCE", "wms-client");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.LoginAsync("nonexistent", "password123"));
    }
}