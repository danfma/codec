using Codec.Core.AST;
using Codec.Core.Parsing;

namespace Codec.Core.Tests.Parsing;

public class ConstraintParserTests
{
    [Fact]
    public void ParseConstraintBlock_Empty_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type TestType = String {}";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];
        typeAlias.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseConstraintBlock_WithWhitespace_ShouldParseCorrectly()
    {
        // Arrange
        var input = @"type TestType = String {
    
}";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];
        typeAlias.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseConstraintBlock_WithSingleConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type TestType = String { constraint min_len(1) }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];
        typeAlias.Annotations.Length.ShouldBe(1);

        var constraint = typeAlias.Annotations[0];
        constraint.ShouldBeOfType<Min>();
        var minConstraint = (Min)constraint;
        minConstraint.Value.ShouldBe(1);
    }

    [Fact]
    public void ParseConstraintBlock_WithSingleMultilineConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input =
            @"type TestType = String {
    constraint min_len(1)
}";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];
        typeAlias.Annotations.Length.ShouldBe(1);

        var minConstraint = typeAlias.Annotations.OfType<Min>().First();
        minConstraint.Value.ShouldBe(1);
    }
}