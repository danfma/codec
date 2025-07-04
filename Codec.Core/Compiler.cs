namespace Codec.Core;

public sealed class Compiler(CompilerOptions options) { }

public sealed class CompilerOptions
{
    public string OutputPath { get; init; } = "./output";
    public LanguageTarget LangTarget { get; init; } = LanguageTarget.CSharp;
    public WordConvention TableName { get; init; } = WordConvention.SnakeCase;
    public WordConvention ColumnName { get; init; } = WordConvention.SnakeCase;
    public WordConvention FunctionName { get; init; } = WordConvention.SnakeCase;
}

public enum WordConvention
{
    SnakeCase,
    CamelCase,
    PascalCase,
}

public enum LanguageTarget
{
    CSharp,
}
