using Bunit;
using Bunit.TestDoubles;
using WebAppWithHighSecurity.Components.Account.Pages;

namespace WebAppWithHighSecurity.Tests;

public class AuthorizationTests
{
    [Fact]
    public void UserHasAdminRights()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("admin-user");
        authContext.SetRoles("Admin");

        // Act
        var cut = ctx.RenderComponent<Admin>();

        // Assert
        cut.MarkupMatches("<h1>Admin Page</h1><p>Welcome, admin-user!</p>");
    }

    [Fact]
    public void UserDoesNotHaveAdminRights()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("regular-user");
        authContext.SetRoles("User");

        // Act
        var cut = ctx.RenderComponent<Admin>();

        // Assert
        cut.MarkupMatches("<h1>Access Denied</h1><p>You do not have the necessary permissions to view this page.</p>");
    }

    [Fact]
    public void AccessAdminFunctionsOnRoleChange()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("user");
        authContext.SetRoles("User");

        // Act
        authContext.SetRoles("Admin");
        var cut = ctx.RenderComponent<Admin>();

        // Assert
        cut.MarkupMatches("<h1>Admin Page</h1><p>Welcome, user!</p>");
    }
}