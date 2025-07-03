using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record QueryBlock(
    ImmutableArray<FromClause> FromClauses,
    ImmutableArray<WhereClause> WhereClauses,
    SelectClause SelectClause
) : QueryExpression;
