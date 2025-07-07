using Codec.Core.AST;
using Codec.Core.Parsing;

namespace Codec.Core.Tests.Parsing;

public class TypeAliasWithConstraintsParserTests
{
    [Fact]
    public void ParseTypeAlias_WithConstraintBlock_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type Email = String { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];

        typeAlias.Name.ShouldBe("Email");
        typeAlias.UnderlyingType.ShouldBe(TypeInfo.String);
        typeAlias.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseTypeAlias_WithSingleConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type Cpf = String { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];

        typeAlias.Name.ShouldBe("Cpf");
        typeAlias.UnderlyingType.ShouldBe(TypeInfo.String);
        typeAlias.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseTypeAlias_WithEmptyConstraintBlock_ShouldParseCorrectly()
    {
        // Arrange
        var input = @"type SimpleType = String { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];

        typeAlias.Name.ShouldBe("SimpleType");
        typeAlias.UnderlyingType.ShouldBe(TypeInfo.String);
        typeAlias.Annotations.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseTypeAlias_Int32WithConstraint_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type UserId = String { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];

        typeAlias.Name.ShouldBe("UserId");
        typeAlias.UnderlyingType.ShouldBe(TypeInfo.String);
        typeAlias.Annotations.Length.ShouldBe(0);
    }
}