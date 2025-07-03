using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record TransactionStatement(ImmutableArray<CodecStatement> Body) : CodecStatement;
