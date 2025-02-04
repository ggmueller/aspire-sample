using AspireSampleWeb.Crm;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SampleTests;

public class SampleWebApplicationFactory(Dictionary<string, string?>? additionalConfiguration) : WebApplicationFactory<CrmContext>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("integrationtests");

        builder.ConfigureAppConfiguration(
                cb => cb.AddInMemoryCollection(additionalConfiguration));
        return base.CreateHost(builder);
    }
}