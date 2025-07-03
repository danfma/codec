using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record UniqueConstraint(params ImmutableArray<string> ColumnNames) : Constraint;
