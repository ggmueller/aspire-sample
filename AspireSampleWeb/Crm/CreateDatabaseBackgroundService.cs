using Npgsql;
using Polly;
using Polly.Retry;

namespace AspireSampleWeb.Crm;

public class CreateDatabaseBackgroundService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var dbContext = sp.GetRequiredService<CrmContext>();
        
        var resiliencePipeline = new ResiliencePipelineBuilder().AddRetry(new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<NpgsqlException>(e => e.IsTransient),
            BackoffType = DelayBackoffType.Linear,
            MaxRetryAttempts = 10,
        })
        .Build();

        await resiliencePipeline.ExecuteAsync(async ct => await dbContext.Database.EnsureCreatedAsync(ct),
            CancellationToken.None);
    }
}