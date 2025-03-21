using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppWithHighSecurity.Components;
using WebAppWithHighSecurity.Components.Account;
using WebAppWithHighSecurity.Data;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Detect if running inside Docker
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Select connection strings dynamically
string connectionString = (isDocker
    ? builder.Configuration.GetConnectionString("DockerConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection"))!;

string todoConnectionString = (isDocker
    ? builder.Configuration.GetConnectionString("DockerTodoConnection")
    : builder.Configuration.GetConnectionString("TodoConnection"))!;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ISymmetricEncryptionService, SymmetricEncryptionService>();
builder.Services.AddScoped<IAsymmetricEncryptionService, AsymmetricEncryptionService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Register HttpClient with BaseAddress
builder.Services.AddHttpClient("ApiClient", client => { client.BaseAddress = new Uri("https://localhost:7092"); });

// Configure database connections
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<CprTodoDbContext>(options =>
    options.UseSqlServer(todoConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<IdentityRole>()
    .AddSignInManager()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddDefaultTokenProviders();

builder.Services
    .AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>>();
builder.Services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole, ApplicationDbContext>>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/CprNr");
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToReturnUrl = context =>
    {
        context.Response.Redirect("/CprNr");
        return Task.CompletedTask;
    };
});

// Set up HTTPS with certificate inside Docker
var certPath = "/https/MyCert.pfx";
var passwordPath = "/https/password.txt";

if (isDocker)
{
    if (!File.Exists(passwordPath))
    {
        throw new FileNotFoundException($"Certifikat-password filen blev ikke fundet: {passwordPath}");
    }

    var certPassword = File.ReadAllText(passwordPath).Trim();
    var cert = new X509Certificate2(certPath, certPassword);

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8081, listenOptions => { listenOptions.UseHttps(cert); });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();