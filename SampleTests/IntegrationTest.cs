using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using AspireSampleWeb.Crm;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Polly;
using Polly.Retry;

namespace SampleTests;

public class IntegrationTest : IAsyncLifetime
{
    private IDistributedApplicationTestingBuilder _appHost = null!;
    private DistributedApplication _app = null!;
    private WebApplicationFactory<CrmContext> _webApplicationFactory = null!;
    private HttpClient _httpClient = null!;

    public async Task InitializeAsync()
    {
        _appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireSampleWeb>();

        var webProject = _appHost.Resources.First(r => r.Name == "sample");

        _appHost.Resources.Remove(webProject);

        var databaseResource = _appHost.Resources.OfType<IResourceWithConnectionString>().First(r => r.Name == "db");
        var containerMountAnnotation = databaseResource.Annotations.OfType<ContainerMountAnnotation>().First();
        databaseResource.Annotations.Remove(containerMountAnnotation);

        _app = await _appHost.BuildAsync();
        await _app.StartAsync();
        
        // _httpClient = _app.CreateHttpClient("sample");

        await WaitForDatabaseReady();
        
        
        var webApplicationConfiguration = 
            await webProject.LoadConfigurationAsync(_app.Services);

        _webApplicationFactory = new SampleWebApplicationFactory(webApplicationConfiguration);

        _httpClient = _webApplicationFactory.CreateClient();
        
        async Task WaitForDatabaseReady()
        {
            var retryLoad = new ResiliencePipelineBuilder().AddRetry(new RetryStrategyOptions()
            {
                MaxRetryAttempts = 20,
                Delay = TimeSpan.FromMilliseconds(200)
            }).Build();

            var databaseConnectionString = await databaseResource.GetConnectionStringAsync();
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(databaseConnectionString)
            {
                Database = "postgres"
            };
            var masterDbConnectionString = connectionStringBuilder.ToString();

            await retryLoad.ExecuteAsync(
                async _ => {
                    await using (var connection = new NpgsqlConnection(masterDbConnectionString))
                    {
                        await connection.OpenAsync();
                        retryLoad.Execute(() => new NpgsqlCommand("SELECT 1", connection).ExecuteScalar());
                    }
                });
        }

    }

    public async Task DisposeAsync()
    {
        await _app.DisposeAsync();
        await _webApplicationFactory.DisposeAsync();
    }

    [Fact]
    public async Task Test()
    {
        var response = await _httpClient.PostAsJsonAsync("/api/deals", new Deal(){Name = "test", Price = 10});
        response.EnsureSuccessStatusCode();
        var deals = await _httpClient.GetFromJsonAsync<List<Deal>>("/api/deals");
        Assert.NotEmpty(deals);
    }
}