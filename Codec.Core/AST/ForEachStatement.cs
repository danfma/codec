using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record ForEachStatement(
    string ItemName,
    string Collection,
    ImmutableArray<CodecStatement> Body
) : CodecStatement;
