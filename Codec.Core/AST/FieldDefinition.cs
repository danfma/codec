using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record FieldDefinition(
    string Name,
    TypeInfo Type,
    bool IsMutable,
    params ImmutableArray<Annotation> Annotations
);
