using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbPassword = builder.AddParameter("dbpwd", "DbPassword", secret: true);

var postgresDb = builder.AddPostgres("db", password: dbPassword, port: 15432);
var sampleDb = postgresDb.AddDatabase("sampledb", "sample");

var smtp = builder.AddContainer("smtp", "docker.io/mailhog/mailhog")
    .WithEndpoint(name: "smtp-endpoint", port: 1025, targetPort: 1025)
    .WithHttpEndpoint(8025, 8025);

builder.AddProject<AspireSampleWeb>("sample")
    .WithReference(sampleDb)
    .WithReference(smtp.GetEndpoint("smtp-endpoint"));

builder.Build().Run();