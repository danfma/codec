using Codec.Core.AST;
using Codec.Core.Generators;
using Codec.Core.Parsing;

namespace Codec.Cli.Commands;

public sealed class CodeCommand(ILogger<CodeCommand> logger)
{
    /// <summary>
    /// Loads the schema configuration and generates code targeting the specified language, creating all entities,
    /// auxiliary types and services.
    /// </summary>
    /// <param name="input">-i, The input file</param>
    /// <param name="template">-t, The template to be used (Default is "csharp-with-dapper")</param>
    /// <param name="output">-o, The output directory</param>
    /// <param name="cancellationToken"></param>
    public async Task Generate(
        string input,
        string template = "csharp-with-dapper",
        string output = "./generated",
        CancellationToken cancellationToken = default
    )
    {
        if (!ResolveGenerator(template, out var generator))
            return;

        if (!ResolveInputFile(input, out var inputFile))
            return;

        if (!GetOutputDirectory(output, out var outputDir))
            return;

        if (!TryParseCodecProject(inputFile, out var project))
            return;

        logger.ZLogInformation($"Generating code using template {generator.DisplayName}");
        logger.ZLogInformation($"\t- input:  {inputFile}");
        logger.ZLogInformation($"\t- output: {outputDir}");

        await generator.GenerateAsync(project, outputDir, cancellationToken);
    }

    private bool ResolveGenerator(
        string templateName,
        [NotNullWhen(true)] out IOutputGenerator? generator
    )
    {
        generator = OutputGeneratorFactory.CreateFromTemplate(templateName);

        if (generator is null)
        {
            Console.WriteLine(Chalk.Red + $"Template '{templateName}' is not supported.");
        }

        return generator is not null;
    }

    private bool ResolveInputFile(string input, [NotNullWhen(true)] out FileInfo? file)
    {
        file = new FileInfo(Path.GetRelativePath(Environment.CurrentDirectory, input));

        if (!file.Exists)
        {
            Console.WriteLine(Chalk.Red + $"Schema file '{input}' was not found.");
            return false;
        }

        return true;
    }

    private bool GetOutputDirectory(
        string directory,
        [NotNullWhen(true)] out DirectoryInfo? outputDir
    )
    {
        outputDir = new DirectoryInfo(
            Path.GetRelativePath(Environment.CurrentDirectory, directory)
        );

        if (!outputDir.Exists)
        {
            outputDir.Create();
        }

        return true;
    }

    private bool TryParseCodecProject(
        FileInfo schemaFile,
        [NotNullWhen(true)] out CodecProject? project
    )
    {
        var parsed = true;

        try
        {
            var fileContent = File.ReadAllText(schemaFile.FullName, Encoding.UTF8);

            project = CodecParser.Parse(fileContent);
        }
        catch (Exception exception)
        {
            Console.WriteLine(Chalk.Red + "Could not parse codec schema");

            logger.ZLogError(
                exception,
                $"Failed to parse codec schema from file '{schemaFile.FullName}'"
            );

            project = null;
            parsed = false;
        }

        return parsed;
    }
}
