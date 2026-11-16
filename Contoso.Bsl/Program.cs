using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCertificateAuthorization(builder);
builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services
    .AddSqlServerDatabaseConfiguration(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddLogging()
    .AddContosoBslFlowServices()
    .AddAutoMapperConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
