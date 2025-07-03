using System.Collections.Immutable;

namespace Codec.Core.AST;

public sealed record EnumDefinition(string Name, ImmutableArray<EnumValue> Values);
