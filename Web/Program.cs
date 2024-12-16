using Microsoft.EntityFrameworkCore;
using Persistence;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.Controllers.KeyMappingController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbDependencies(builder.Configuration);
builder.Services.AddServiceDependencies(builder.Configuration);

var app = builder.Build();

ApplyMigrations(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var keyMappingDbContext = services.GetRequiredService<KeyMappingDbContext>();
        keyMappingDbContext.Database.Migrate();

        var shardConfiguration = services.GetRequiredService<IShardConfiguration>();
        var shardDbContextFactory = services.GetRequiredService<IShardDbContextFactory>();

        foreach (var connectionString in shardConfiguration.GetShardConnectionStrings())
        {
            using (var shardDbContext = shardDbContextFactory.Create(connectionString))
            {
                shardDbContext.Database.Migrate();
            }
        }
    }
}