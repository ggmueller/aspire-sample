using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var dbInstance = builder.AddPostgres("dbi")
    .WithDataVolume();
var crmDb = dbInstance.AddDatabase("crmdb", "crm")
    ;



var smtp = builder.AddContainer("smtp", "docker.io/mailhog/mailhog")
    .WithEndpoint(1025, 1025, name: "smtp")
    .WithHttpEndpoint(8025, 8025);

builder.AddProject<AspireSampleWeb>("crm")
    .WithReference(crmDb)
    .WithReference(smtp.GetEndpoint("smtp"));

builder.Build().Run();