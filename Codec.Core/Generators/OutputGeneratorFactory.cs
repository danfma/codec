using Codec.Core.Generators.CSharp;

namespace Codec.Core.Generators;

public static class OutputGeneratorFactory
{
    public static IOutputGenerator? CreateFromTemplate(string templateName)
    {
        return templateName.ToLowerInvariant() switch
        {
            "csharp-with-dapper" => new CSharpWithDapperTemplate(),
            _ => null,
        };
    }
}
