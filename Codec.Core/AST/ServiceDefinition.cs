using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record ServiceDefinition(string Name, ImmutableArray<FunctionDefinition> Functions);
