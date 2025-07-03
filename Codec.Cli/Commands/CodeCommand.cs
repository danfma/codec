using Codec.Core;
using Microsoft.Extensions.Logging;

namespace Codec.Cli.Commands;

public sealed class CodeCommand(ILogger<CodeCommand> logger)
{
    /// <summary>
    /// Loads the schema configuration and generates code targeting the specified language, creating all entities,
    /// auxiliary types and services.
    /// </summary>
    /// <param name="language">-lang, The target language</param>
    /// <param name="output">-o, The output directory</param>
    /// <returns></returns>
    public Task Generate(LanguageTarget language, string output = "./output")
    {
        logger.ZLogInformation($"Generating code");
        logger.ZLogInformation($"\tTarget: {language}");

        return Task.CompletedTask;
    }
}
