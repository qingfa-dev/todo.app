IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
    .WithPgWeb();

IResourceBuilder<PostgresDatabaseResource> database = postgres
    .AddDatabase("PostgresConnection");

builder
    .AddProject<Projects.Todo_Api>("api")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
