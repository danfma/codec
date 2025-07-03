namespace Codec.Core.AST;

public sealed record VarStatement(string VariableName, string Expression) : CodecStatement;
