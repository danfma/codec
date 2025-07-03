using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record IndexConstraint(params ImmutableArray<string> ColumnNames) : Constraint;
