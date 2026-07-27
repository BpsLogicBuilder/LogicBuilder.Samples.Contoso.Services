using Contoso.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SecureCorsPolicy", policy =>
    {
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .WithMethods("GET", "POST", "PUT", "DELETE");
        }
    });
});
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

app.UseCors("SecureCorsPolicy");

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
