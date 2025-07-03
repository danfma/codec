using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record ViewDefinition(
    string Name,
    ImmutableArray<TraitRef> ImplementedTraits,
    ImmutableArray<FieldDefinition> Fields,
    QueryExpression Query
);
