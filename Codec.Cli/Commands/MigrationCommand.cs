namespace Codec.Cli.Commands;

public class MigrationCommand(ILogger<MigrationCommand> logger)
{
    public Task Add(string name)
    {
        logger.ZLogInformation($"Adding migration: {name}");

        return Task.CompletedTask;
    }

    public Task Remove(string name)
    {
        logger.ZLogInformation($"Removing migration: {name}");

        return Task.CompletedTask;
    }
}
