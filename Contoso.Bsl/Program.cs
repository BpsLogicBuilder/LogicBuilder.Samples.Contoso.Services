using LogicBuilder.App.Utils.Json;
using LogicBuilder.Domain.Json;
using LogicBuilder.Expressions.Utils.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCertificateAuthorization(builder);

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
