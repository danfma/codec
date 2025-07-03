using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record AbstractEntityDefinition(
    string Name,
    ImmutableArray<TraitRef> ImplementedTraits,
    ImmutableArray<FieldDefinition> Fields,
    InheritanceStrategy InheritanceStrategy
);
