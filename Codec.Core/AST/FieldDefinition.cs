using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record FieldDefinition(
    string Name,
    TypeInfo Type,
    params ImmutableArray<Annotation> Annotations
);
