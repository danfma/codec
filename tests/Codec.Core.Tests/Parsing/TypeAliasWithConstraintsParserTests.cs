using Codec.Core.AST;
using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class TypeAliasWithConstraintsParserTests
{
    [Fact]
    public void ParseTypeAlias_WithConstraintBlock_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type Email = String { }";

        // Act
        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Email");
        result.UnderlyingType.ShouldBe(TypeInfo.String);
        result.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseTypeAlias_WithSingleConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type Cpf = String { }";

        // Act
        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Cpf");
        result.UnderlyingType.ShouldBe(TypeInfo.String);
        result.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseTypeAlias_WithEmptyConstraintBlock_ShouldParseCorrectly()
    {
        // Arrange
        var input = @"type SimpleType = String { }";

        // Act
        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("SimpleType");
        result.UnderlyingType.ShouldBe(TypeInfo.String);
        result.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseTypeAlias_Int32WithConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type UserId = String { }";

        // Act
        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("UserId");
        result.UnderlyingType.ShouldBe(TypeInfo.String);
        result.Annotations.Length.ShouldBe(0);
    }
}
