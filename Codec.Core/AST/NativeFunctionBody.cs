namespace Codec.Core.AST;

public sealed record NativeFunctionBody(string Dialect, string DatabaseType, string Code)
    : FunctionBody;
