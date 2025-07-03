namespace Codec.Core.AST;

public sealed record InheritanceStrategy(
    InheritanceStrategyType Strategy,
    string? DiscriminatorColumn = null
);
