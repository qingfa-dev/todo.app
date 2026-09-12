IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
    .WithPgWeb();

IResourceBuilder<PostgresDatabaseResource> database = postgres
    .AddDatabase("PostgresConnection");

IResourceBuilder<ProjectResource> api = builder
    .AddProject<Projects.Todo_Api>("api")
    .WithReference(database)
    .WaitFor(database);

builder
    .AddViteApp("web", "../Web", "dev")
    .WithPnpm()
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
