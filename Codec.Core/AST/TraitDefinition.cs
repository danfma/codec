using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record TraitDefinition(string Name, params ImmutableHashSet<FieldDefinition> Fields)
{
    public ImmutableArray<TraitRef> Extends { get; init; } = ImmutableArray<TraitRef>.Empty;
}
