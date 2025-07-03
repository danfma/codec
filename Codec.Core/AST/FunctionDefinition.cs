using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record FunctionDefinition(
    string Name,
    ImmutableArray<ParameterDefinition> Parameters,
    TypeInfo? ReturnType,
    FunctionBody Body
);
