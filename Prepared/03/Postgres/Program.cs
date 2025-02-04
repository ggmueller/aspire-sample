using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("dbpwd", "DbPassword", secret: true);

var postgresDb = builder.AddPostgres("db", password: dbPassword, port: 15432);
builder.AddProject<AspireSampleWeb>("sample")
    .WithReference(postgresDb);

builder.Build().Run();