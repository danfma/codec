using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record TypeAlias(
    string Name,
    TypeInfo UnderlyingType,
    params ImmutableArray<Annotation> Annotations
);
