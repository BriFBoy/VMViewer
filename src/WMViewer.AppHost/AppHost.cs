using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var api = builder.AddProject<WMViewer_Api>("api")
    .WithHttpHealthCheck("/health")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();