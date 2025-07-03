using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record IfStatement(
    string Condition,
    ImmutableArray<CodecStatement> ThenStatements,
    ImmutableArray<CodecStatement> ElseStatements
) : CodecStatement;
