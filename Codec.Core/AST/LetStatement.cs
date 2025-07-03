namespace Codec.Core.AST;

public sealed record LetStatement(string VariableName, string Expression) : CodecStatement;
