using Scalar.AspNetCore;

namespace Todo.Api.Infrastructure.OpenApi;

public static class OpenApiExtensions
{

    public static WebApplicationBuilder AddOpenApiDocumentation(
        this WebApplicationBuilder builder)
    {

        builder.Services.AddOpenApi(options =>
            options.ConfigureCustomOptions());


        return builder;
    }



    public static WebApplication UseOpenApiDocumentation(this WebApplication app)
    {

        app.MapOpenApi();
        app.MapScalarApiReference(
            options =>
            {
                options.WithTitle("Todo API")
                .WithClassicLayout();
            });


        return app;
    }
}
