
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);


// JWT
var jwt = builder.Configuration.GetSection("JwtSettings");
var issuer = jwt["Issuer"];
var audience = jwt["Audience"];
var secret = jwt["Secret"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!))
        };
        // forward token to downstream automatically
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                // keep Authorization header as-is; YARP forwards it.
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// simple fixed window rate limit (per-IP)
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 100;            // 100 req
        options.Window = TimeSpan.FromMinutes(1); // per minute
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    }));

// CORS for local dev
builder.Services.AddCors(o =>
    o.AddPolicy("dev", p =>
        p.WithOrigins("http://localhost:5173", "http://localhost:4200", "http://localhost:3000")
         .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Pass-through Swagger from services via gateway paths
        c.SwaggerEndpoint("/identity/swagger/v1/swagger.json", "Identity");
        c.SwaggerEndpoint("/catalog/swagger/v1/swagger.json", "Catalog");
        c.SwaggerEndpoint("/cart/swagger/v1/swagger.json", "Cart");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("dev");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/_health", () => Results.Ok("gateway-ok")).AllowAnonymous();

app.MapReverseProxy(); // routes come from appsettings

app.Run();