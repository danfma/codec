namespace Codec.Core.AST;

public sealed record Regex(string Pattern) : Annotation("regex");
