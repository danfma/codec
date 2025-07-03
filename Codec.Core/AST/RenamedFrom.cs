namespace Codec.Core.AST;

public sealed record RenamedFrom(string OldName) : Annotation("renamedFrom");
