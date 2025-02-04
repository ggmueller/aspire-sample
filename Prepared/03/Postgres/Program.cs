using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("dbpwd", "DbPassword", secret: true);

builder.AddProject<AspireSampleWeb>("sample");

builder.AddPostgres("db", password: dbPassword, port: 15432);

builder.Build().Run();