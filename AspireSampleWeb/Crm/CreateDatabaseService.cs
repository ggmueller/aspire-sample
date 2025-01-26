namespace AspireSampleWeb.Crm;

public class CreateDatabaseService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var sp = scope.ServiceProvider;

        var dbContext = sp.GetRequiredService<CrmContext>();
        await dbContext.Database.EnsureCreatedAsync(CancellationToken.None);
    }
}