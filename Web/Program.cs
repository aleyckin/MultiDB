using Microsoft.EntityFrameworkCore;
using Persistence;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.Controllers.KeyMappingController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<KeyMappingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("KeyMappingDb")));

builder.Services.AddDbContext<ShardDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ShardDb1")));

builder.Services.AddDbContext<ShardDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ShardDb2")));

builder.Services.AddSingleton<List<string>>(sp =>
    new List<string>
    {
        builder.Configuration.GetConnectionString("ShardDb1"),
        builder.Configuration.GetConnectionString("ShardDb2")
    });

builder.Services.AddServiceDependencies();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var keyMappingDbContext = services.GetRequiredService<KeyMappingDbContext>();
    keyMappingDbContext.Database.Migrate();

    var shardDbContexts = services.GetServices<ShardDbContext>();
    foreach (var shardDbContext in shardDbContexts)
    {
        shardDbContext.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
