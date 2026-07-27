using Contoso.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddControllers().AddJsonOptions
(
    options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = SerializationOptions.Default.PropertyNameCaseInsensitive;
        foreach(var converter in SerializationOptions.Default.Converters)
            options.JsonSerializerOptions.Converters.Add(converter);
    }
);

builder.Services.AddHttpClient();
builder.Services.AddAppUtilsHttpClientHelper();
builder.Services.Configure<UrlOptions>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
