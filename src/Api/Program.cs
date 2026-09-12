using System.Text.Json;

using Carter;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Todo.Api.Infrastructure.IAM;
using Todo.Api.Infrastructure.OpenApi;
using Todo.Api.Infrastructure.Persistence;
using Todo.Api.SharedKernel.Extensions;
using Todo.Api.SharedKernel.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationPersistence();
builder.AddApplicationIdentity();
builder.AddApplicationAuthentication();
builder.Services.AddAuthorizationBuilder();

builder.Services.AddCarter();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<
    Todo.Api.Features.Todos.TodoLists.CreateTodoList.Validator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureHttpJsonOptions(
    options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.PropertyNameCaseInsensitive = true;
        options.SerializerOptions.Converters.Add(new ResultConverterFactory());
    });

builder.Services.AddOpenApi(options =>
    options.ConfigureCustomOptions());

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApiDocumentation();

    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapCarter();

app.Run();
