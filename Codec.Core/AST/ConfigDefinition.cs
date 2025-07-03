using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record ConfigDefinition(ImmutableDictionary<string, string> Settings);
