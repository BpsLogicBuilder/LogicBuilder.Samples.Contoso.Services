using LogicBuilder.App.Utils.Json;
using LogicBuilder.Domain.Json;
using LogicBuilder.Expressions.Utils.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions
(
    options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DescriptorConverter());
        options.JsonSerializerOptions.Converters.Add(new ModelConverter());
        options.JsonSerializerOptions.Converters.Add(new ObjectConverter());
    }
);

builder.Services
    .AddSqlServerDatabaseConfiguration(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddLogging()
    .AddContosoBslFlowServices()
    .AddAutoMapperConfiguration()
    .AddDynamicRulesLoaderConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
