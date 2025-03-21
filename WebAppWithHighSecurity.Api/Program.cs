using WebAppWithHighSecurity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAsymmetricEncryptionService, AsymmetricEncryptionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppWithHighSecurity API V1");
    });
}

app.UseHttpsRedirection();

app.MapPost("/AsymmetricEncryption/Encrypt", ([FromBody] EncryptionRequest request) =>
    {
        using (var rsa = RSA.Create())
        {
            rsa.FromXmlString(request.PublicKey);
            var encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(request.PlainText), RSAEncryptionPadding.OaepSHA256);
            return Results.Ok(Convert.ToBase64String(encryptedBytes));
        }
    })
    .WithName("Encrypt")
    .WithOpenApi();

app.Run();

record EncryptionRequest(string PublicKey, string PlainText);