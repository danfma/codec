using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record CodecFunctionBody(ImmutableArray<CodecStatement> Statements) : FunctionBody;
