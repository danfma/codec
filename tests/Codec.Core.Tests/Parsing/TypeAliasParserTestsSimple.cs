using Codec.Core.AST;
using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class TypeAliasParserTestsSimple
{
    [Fact]
    public void ParseTypeAlias_SimpleTypeAlias_ShouldParseCorrectly()
    {
        // Arrange
        var input = "type Email = String";

        // Act
        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Email");
        result.UnderlyingType.ShouldBe(TypeInfo.String);
        result.Annotations.ShouldBeEmpty();
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
        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe(expectedName);
        result.UnderlyingType.Name.ShouldBe(expectedTypeName);
        result.Annotations.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("type lowercase = String", "Type names must use PascalCase")]
    [InlineData("type Email = UnknownType", "Unknown primitive type")]
    [InlineData("type Email String", "Missing equals sign")]
    [InlineData("type = String", "Missing type name")]
    [InlineData("typeEmail = String", "Missing space after keyword")]
    public void ParseTypeAlias_InvalidSyntax_ShouldFail(string input, string reason)
    {
        // Act
        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        // Assert
        parseResult.Success.ShouldBeFalse($"Should fail for: {reason}");
    }
}
