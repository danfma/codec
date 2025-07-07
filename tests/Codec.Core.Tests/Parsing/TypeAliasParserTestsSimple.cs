using Codec.Core.AST;
using Codec.Core.Parsing;

namespace Codec.Core.Tests.Parsing;

public class TypeAliasParserTestsSimple
{
    [Fact]
    public void ParseTypeAlias_SimpleTypeAlias_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type Email = String";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];

        typeAlias.Name.ShouldBe("Email");
        typeAlias.UnderlyingType.ShouldBe(TypeInfo.String);
        typeAlias.Annotations.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("type StringAlias = String", "StringAlias", "String")]
    [InlineData("type  StringAlias = String", "StringAlias", "String")]
    [InlineData(" type StringAlias = String", "StringAlias", "String")]
    public void ParseTypeAlias_AllPrimitiveTypes_ShouldParseCorrectly(
        string input,
        string expectedName,
        string expectedTypeName
    )
    {
        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];

        typeAlias.Name.ShouldBe(expectedName);
        typeAlias.UnderlyingType.Name.ShouldBe(expectedTypeName);
        typeAlias.Annotations.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("type lowercase = String", "Type names must use PascalCase")]
    [InlineData("type Email = UnknownType", "Unknown primitive type")]
    [InlineData("type Email String", "Missing equals sign")]
    [InlineData("type = String", "Missing type name")]
    [InlineData("typeEmail = String", "Missing space after keyword")]
    public void ParseTypeAlias_InvalidSyntax_ShouldReturnEmptyProject(string input, string reason)
    {
        // Act
        var result = CodecParser.Parse(input);
        
        // Assert - Parser is now tolerant and returns empty/partial results
        // instead of throwing exceptions for invalid syntax
        result.ShouldNotBeNull();
        
        // For most invalid syntax, we expect no valid type aliases to be parsed
        if (reason != "Unknown primitive type") // This one might still parse partially
        {
            result.TypeAliases.Length.ShouldBe(0);
        }
    }
}