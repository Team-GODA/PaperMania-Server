using Azure.Identity;
using Server.Api.Extensions;
using Server.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["Azure:KeyVaultName"] 
                   ?? "papermaniadbconnection";
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

var redisConnectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration["Redis:Development"]
    : builder.Configuration["Redis:Production"];

builder.Services
    .AddCache(redisConnectionString!)
    .AddApplicationServices()
    .AddRepositories();

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

app.UseSwaggerConfiguration()
    .UseCustomMiddleware()
    .UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();

await InitializeCachesAsync(app.Services);

app.Run();

static async Task InitializeCachesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    
    var stageRewardCache = scope.ServiceProvider.GetRequiredService<StageRewardCache>();
    var characterDataCache = scope.ServiceProvider.GetRequiredService<CharacterDataCache>();
    
    await Task.WhenAll(
        stageRewardCache.Initialize(),
        characterDataCache.Initialize()
    );
}
