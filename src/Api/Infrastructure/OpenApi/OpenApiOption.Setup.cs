using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Todo.Api.Infrastructure.OpenApi;

public static class OpenApiOptionsSetup
{

    public static OpenApiOptions ConfigureCustomOptions(this OpenApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.CreateSchemaReferenceId = jsonTypeInfo =>
        {
            string? defaultId = OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo);
            if (defaultId is null)
            {
                return null;
            }

            return OpenApiSchemaNaming.GetSchemaReferenceId(jsonTypeInfo.Type);
        };

        options.AddDocumentTransformer((document, _, _) =>
        {
            // Add: Bearer JWT security scheme so Scalar shows the "Authorize" button
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT Bearer token"
            };

            document.Security =
            [
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", null, null)] = []
                }
            ];

            return Task.CompletedTask;
        });

        return options;
    }
}
