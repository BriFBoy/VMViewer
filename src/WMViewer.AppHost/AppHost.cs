using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var database = postgres.AddDatabase("wmviewer");

var api = builder.AddProject<WMViewer_Api>("api")
    .WithHttpHealthCheck("/health")
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<WMViewer_DataSync>("datasync")
    .WithReference(database)
    .WaitFor(database)
    .WithEnvironment("connection__string", database.Resource.ConnectionStringExpression);

builder.Build().Run();