using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record PrimaryKeyConstraint(params ImmutableArray<string> ColumnNames) : Constraint;
