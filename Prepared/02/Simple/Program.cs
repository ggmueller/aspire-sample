using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<AspireSampleWeb>("sample");

builder.Build().Run();