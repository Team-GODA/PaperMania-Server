using Azure.Identity;
using Server.Api.Extensions;
using Server.Api.Middleware;
using Server.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["Azure:KeyVaultName"] 
                   ?? "papermaniadbconnection";
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

var redisConnectionString = builder.Configuration["Redis:ConnectionString"];

builder.Services.AddHttpClient();

builder.Services
    .AddCache(redisConnectionString!)
    .AddStaticDataStores()
    .AddApplicationServices()
    .AddRepositories()
    .AddApiFilters();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v3",
        Title = "PaperMania API",
        Description = "API Version"
    });
});

var app = builder.Build();

app.UseSwaggerConfiguration();

app.UseCors("ClientPolicy");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCustomMiddleware();

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
