using Carter;

using FluentValidation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Todo.Api.Domain.IAM.Entities;
using Todo.Api.Features.IAM.Services;
using Todo.Api.SharedKernel.Data;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

namespace Todo.Api.Features.IAM;

public sealed class IAMModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/auth/register",
                async (
                    Register.Request request,
                    UserManager<ApplicationUser> userManager,
                    IValidator<Register.Request> validator,
                    CancellationToken cancellationToken) =>
                {
                    Result<Register.Response> result =
                        await Register.Handler.Handle(
                            request,
                            userManager,
                            validator,
                            cancellationToken);

                    return result.ToHttpResult();
                })
            .WithTags("Authentication")
            .AllowAnonymous();

        app.MapPost(
                "/api/auth/login",
                async (
                    Login.Request request,
                    UserManager<ApplicationUser> userManager,
                    SignInManager<ApplicationUser> signInManager,
                    IJwtTokenService tokenService,
                    IApplicationDbContext dbContext,
                    IOptions<JwtOptions> jwtOptions,
                    IValidator<Login.Request> validator,
                    CancellationToken cancellationToken) =>
                {
                    Result<Login.Response> result =
                        await Login.Handler.Handle(
                            request,
                            userManager,
                            signInManager,
                            tokenService,
                            dbContext,
                            jwtOptions,
                            validator,
                            cancellationToken);

                    return result.ToHttpResult();
                })
            .WithTags("Authentication")
            .AllowAnonymous();

        app.MapPost(
               "/api/auth/refresh",
               async (
                   Refresh.Request request,
                   IApplicationDbContext dbContext,
                   IJwtTokenService tokenService,
                   UserManager<ApplicationUser> userManager,
                   IOptions<JwtOptions> jwtOptions,
                   IValidator<Refresh.Request> validator,
                   CancellationToken cancellationToken) =>
               {
                   Result<Refresh.Response> result =
                       await Refresh.Handler.Handle(
                           request,
                           dbContext,
                           tokenService,
                           userManager,
                           jwtOptions,
                           validator,
                           cancellationToken);

                   return result.ToHttpResult();
               })
           .WithTags("Authentication")
           .AllowAnonymous();

        app.MapPost(
               "/api/auth/logout",
               async (
                   Logout.Request request,
                   IApplicationDbContext dbContext,
                   IJwtTokenService tokenService,
                   IValidator<Logout.Request> validator,
                   CancellationToken cancellationToken) =>
               {
                   Result result =
                       await Logout.Handler.Handle(
                           request,
                           dbContext,
                           tokenService,
                           validator,
                           cancellationToken);

                   return result.ToHttpResult();
               })
           .WithTags("Authentication")
           .RequireAuthorization();
    }
}