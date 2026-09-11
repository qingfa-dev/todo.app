using Microsoft.EntityFrameworkCore;

using Todo.Api.Infrastructure.Persistence.Options;
using Todo.Api.SharedKernel.Data;

namespace Todo.Api.Infrastructure.Persistence;

public static class PersistenceExtension
{
    public static WebApplicationBuilder AddApplicationPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {

            var resolvedName = builder.Configuration.GetConnectionString(DatabaseOption.Aspire) != null
                ? DatabaseOption.Aspire
                : DatabaseOption.Default;
            string? connectionString = builder.Configuration.GetConnectionString(resolvedName);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{resolvedName}' is missing. Please provide it in your configuration.");
            }
            options.UseNpgsql(connectionString);
        });

        builder.Services.AddScoped<IApplicationDbContext>(
            provider =>
                provider.GetRequiredService<ApplicationDbContext>());
        return builder;
    }

}