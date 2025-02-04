namespace SampleTests;

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

public static class AspireWebApplicationFactoryExtensions
{
    public static async Task<Dictionary<string, string?>?> LoadConfigurationAsync(this IResource resource, IServiceProvider appHostServiceProvider,
            CancellationToken cancellationToken = default)
    {
        // Use David Fowlers code to bind WebApplication factory to Aspire until it is supported by Aspire: https://github.com/dotnet/aspire/discussions/878#discussioncomment-9631749
        if (resource is IResourceWithEnvironment resourceWithEnvironment && resourceWithEnvironment.TryGetEnvironmentVariables(out var annotations))
        {
            var environmentCallbackContext = new EnvironmentCallbackContext(
                    new DistributedApplicationExecutionContext(
                            new DistributedApplicationExecutionContextOptions(DistributedApplicationOperation.Run)
                            {
                                    ServiceProvider = appHostServiceProvider
                            }),
                    cancellationToken: cancellationToken);
            
            foreach (var annotation in annotations) await annotation.Callback(environmentCallbackContext);

            // Translate environment variable __ syntax to :
            var config = new Dictionary<string, string?>();
            foreach (var (key, value) in environmentCallbackContext.EnvironmentVariables)
            {
                if (resource is ProjectResource && key == "ASPNETCORE_URLS") continue;
                if (resource is ProjectResource && key == "ASPNETCORE_HTTPS_PORT") continue;

                var configValue = value switch
                {
                        string val       => val,
                        IValueProvider v => await v.GetValueAsync(cancellationToken),
                        null             => null,
                        _                => throw new InvalidOperationException($"Unsupported value, {value.GetType()}")
                };

                if (configValue is not null) config[key.Replace("__", ":")] = configValue;
            }

            return config;
        }

        return null;
    }
}