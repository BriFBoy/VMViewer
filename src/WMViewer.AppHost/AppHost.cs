using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithHostPort(5000)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("wmviewer");

var api = builder.AddProject<WMViewer_Api>("api")
    .WithHttpHealthCheck("/health")
    .WithReference(database)
    .WaitFor(database);

var dataSync = builder.AddProject<WMViewer_Datasync>("datasync")
    .WithReference(database)
    .WaitFor(database)
    .WaitFor(api)
    .WithEnvironment("ConnectionStrings__wmviewer", database.Resource.ConnectionStringExpression);

builder.Build().Run();