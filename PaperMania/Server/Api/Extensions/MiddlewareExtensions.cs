namespace Server.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseSwaggerConfiguration
        (this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v3/swagger.json", "PaperMania API V3");
        });

        return app;
    }
}