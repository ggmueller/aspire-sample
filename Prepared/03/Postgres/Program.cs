using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("dbpwd", "DbPassword", secret: true);

var postgresDb = builder.AddPostgres("db", password: dbPassword, port: 15432)
    .WithDataBindMount("db")
    .WithLifetime(ContainerLifetime.Persistent);

var sampleDb = postgresDb.AddDatabase("sampledb", "sample");

builder.AddProject<AspireSampleWeb>("sample")
    .WithReference(sampleDb);

builder.Build().Run();