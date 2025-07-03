using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record EntityDefinition(
    string Name,
    AbstractEntityDefinition? BaseType,
    ImmutableArray<TraitRef> ImplementedTraits,
    ImmutableArray<FieldDefinition> Fields,
    ImmutableArray<RelationshipDefinition> Relationships,
    ImmutableArray<Constraint> Constraints
);
