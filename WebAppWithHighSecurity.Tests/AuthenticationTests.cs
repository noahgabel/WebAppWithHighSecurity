using Bunit;
using Bunit.TestDoubles;
using WebAppWithHighSecurity.Components.Account.Pages;
using WebAppWithHighSecurity.Components.Pages;

namespace WebAppWithHighSecurity.Tests;

public class AuthenticationTests
{
    [Fact]
    public void UserIsAuthenticated()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("test-user");

        // Act
        var cut = ctx.RenderComponent<Auth>();

        // Assert
        cut.MarkupMatches("<h1>You are authenticated</h1><p>Hello test-user!</p>");
    }

    [Fact]
    public void UserIsNotAuthenticated()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetNotAuthorized();

        // Act
        var cut = ctx.RenderComponent<Auth>();

        // Assert
        cut.MarkupMatches("<h1>You are not authenticated</h1>");
    }
    
    [Fact]
    public void SessionTimeoutAndReauthentication()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("test-user");

        // Act
        authContext.SetNotAuthorized();
        authContext.SetAuthorized("test-user");

        // Assert
        var cut = ctx.RenderComponent<Auth>();
        cut.MarkupMatches("<h1>You are authenticated</h1> <p>Hello test-user!</p>");
    }
}