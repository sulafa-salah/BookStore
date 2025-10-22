namespace Catalog.Api.Middlewares;
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseInternalAuth(this IApplicationBuilder app)
            => app.UseMiddleware<InternalAuthMiddleware>();
    }