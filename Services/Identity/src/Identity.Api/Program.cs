using Identity.Api;
using Identity.Application;
using Identity.Infrastructure;
using Identity.Infrastructure.Persistence.Seeding;


var builder = WebApplication.CreateBuilder(args);

// config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>(optional: true);

// ORDER MATTERS: Infrastructure first (adds DbContext, Identity, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// then Application + Presentation
builder.Services.AddApplication();
builder.Services.AddPresentation();

// register seeder
builder.Services.AddSingleton<RbacSeeder>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<RbacSeeder>());


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

app.Run();
