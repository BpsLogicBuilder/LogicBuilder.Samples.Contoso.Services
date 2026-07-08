using Contoso.Api;
using LogicBuilder.App.Utils.Json;
using LogicBuilder.App.Web.Utils;
using LogicBuilder.App.Web.Utils.Interfaces;
using LogicBuilder.Domain.Json;
using LogicBuilder.Expressions.Utils.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
builder.Services.AddTransient<IHttpClientHelper, HttpClientHelper>();
builder.Services.Configure<UrlOptions>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
