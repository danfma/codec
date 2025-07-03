namespace Codec.Core.AST;

public sealed record FromClause(string TableName, string? Alias = null);
