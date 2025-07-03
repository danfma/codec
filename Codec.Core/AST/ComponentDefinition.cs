using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record ComponentDefinition(
    string Name,
    params ImmutableHashSet<FieldDefinition> Fields
);
