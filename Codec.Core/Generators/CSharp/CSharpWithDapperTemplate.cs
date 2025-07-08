using Codec.Core.AST;

namespace Codec.Core.Generators.CSharp;

public sealed class CSharpWithDapperTemplate : IOutputGenerator
{
    public string DisplayName => "C# with Dapper";

    public ValueTask GenerateAsync(
        CodecProject project,
        DirectoryInfo outputDir,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
