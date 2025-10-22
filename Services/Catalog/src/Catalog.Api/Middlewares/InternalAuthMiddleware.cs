namespace Catalog.Api.Middlewares;
    public class InternalAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly ILogger<InternalAuthMiddleware> _logger;

        public InternalAuthMiddleware(RequestDelegate next, IConfiguration config, ILogger<InternalAuthMiddleware> logger)
        {
            _next = next;
            _config = config;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/internal"))
            {
                var expectedKey = _config["InternalApi:Key"];
                var providedKey = context.Request.Headers["X-Internal-Key"].FirstOrDefault();

                if (string.IsNullOrEmpty(expectedKey) || !string.Equals(expectedKey, providedKey))
                {
                    _logger.LogWarning("Unauthorized internal API call to {Path}", context.Request.Path);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized internal call");
                    return;
                }
            }

            await _next(context);
        }
    }