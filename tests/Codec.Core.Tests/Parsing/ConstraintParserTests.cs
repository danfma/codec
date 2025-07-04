using Codec.Core.AST;
using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class ConstraintParserTests
{
    [Fact]
    public void ParseConstraintBlock_Empty_ShouldParseCorrectly()
    {
        // Arrange
        var input = "{}";

        // Act
        var parseResult = CodecParser.ParseConstraintBlock.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseConstraintBlock_WithWhitespace_ShouldParseCorrectly()
    {
        // Arrange
        var input = "{\n    \n}";

        // Act
        var parseResult = CodecParser.ParseConstraintBlock.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseConstraintBlock_WithSingleConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input = "{ constraint min_len(1) }";

        // Act
        var parseResult = CodecParser.ParseConstraintBlock.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Length.ShouldBe(1);

        var constraint = parseResult.Value[0];
        constraint.ShouldBeOfType<Min>();
        var minConstraint = (Min)constraint;
        minConstraint.Value.ShouldBe(1);
    }

    [Fact]
    public void ParseConstraintBlock_WithSingleMultilineConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input =
            @"{
    constraint min_len(1)
}";

        // Act
        var parseResult = CodecParser.ParseConstraintBlock.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Length.ShouldBe(1);

        var minConstraint = parseResult.Value.OfType<Min>().First();
        minConstraint.Value.ShouldBe(1);
    }
}
