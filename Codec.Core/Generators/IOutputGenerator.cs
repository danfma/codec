using Codec.Core.AST;

namespace Codec.Core.Generators;

public interface IOutputGenerator
{
    string DisplayName { get; }

    ValueTask GenerateAsync(
        CodecProject project,
        DirectoryInfo outputDir,
        CancellationToken cancellationToken = default
    );
}
