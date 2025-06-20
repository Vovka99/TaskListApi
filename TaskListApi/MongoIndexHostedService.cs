using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TaskListApi;

public class MongoIndexHostedService(IMongoDatabase database, IOptions<MongoOptions> options, ILogger<MongoIndexHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await MongoIndexInitializer.InitializeIndexesAsync(database, options.Value);
        logger.LogInformation("Mongo indexes ensured.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
