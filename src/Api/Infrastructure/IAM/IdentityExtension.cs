using Microsoft.AspNetCore.Identity;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Infrastructure.Persistence;

namespace Todo.Api.Infrastructure.IAM;

public static class IdentityExtension
{
    public static WebApplicationBuilder AddApplicationIdentity(this WebApplicationBuilder builder)
    {
        builder.Services
        .AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(15);
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        return builder;
    }
}