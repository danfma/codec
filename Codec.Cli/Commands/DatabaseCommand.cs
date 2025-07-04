using Microsoft.Extensions.Logging;

namespace Codec.Cli.Commands;

public sealed class DatabaseCommand(ILogger<DatabaseCommand> logger)
{
    public Task Apply(string name)
    {
        logger.ZLogInformation($"Creating database: {name}");

        return Task.CompletedTask;
    }
}
