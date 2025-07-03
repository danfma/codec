using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record SelectClause(ImmutableArray<SelectField> Fields);
