using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder.AddProject<WMViewer_Api>("api")
    .WithHttpHealthCheck("/health")
    .WithReference(postgres)
    .WaitFor(postgres);

var dataSync = builder.AddProject<WMViewer_Datasync>("datasync")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();