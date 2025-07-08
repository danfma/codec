namespace Codec.Core;

public sealed class CompilerOptions
{
    public string OutputPath { get; init; } = "./output";
    public WordConvention TableName { get; init; } = WordConvention.SnakeCase;
    public WordConvention ColumnName { get; init; } = WordConvention.SnakeCase;
    public WordConvention FunctionName { get; init; } = WordConvention.SnakeCase;
}
