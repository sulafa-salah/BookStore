using Catalog.Api;
using Catalog.Application;  
using Catalog.Infrastructure;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
  //  builder.Configuration
  //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  //.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
  //.AddEnvironmentVariables();
  //  if (builder.Environment.IsDevelopment())
  //  {
  //      builder.Configuration.AddUserSecrets<Program>(optional: true);
  //  }

    builder.Services
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration,builder.Environment);
}
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
if (!app.Environment.IsEnvironment("Testing"))
{
    ApplyMigration();
}

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
