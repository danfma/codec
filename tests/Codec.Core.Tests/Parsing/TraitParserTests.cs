using Codec.Core.AST;
using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class TraitParserTests
{
    [Fact]
    public void ParseTrait_Simple_ShouldParseCorrectly()
    {
        // Arrange
        var input =
            @"trait Named {
    var name: String
}";

        // Act
        var parseResult = CodecParser.ParseTrait.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Named");
        result.Fields.Count.ShouldBe(1);

        var field = result.Fields.First();
        field.Name.ShouldBe("name");
        field.Type.ShouldBe(TypeInfo.String);
        field.IsMutable.ShouldBeTrue(); // var means mutable
    }

    [Fact]
    public void ParseTrait_WithSingleField_ShouldParseCorrectly()
    {
        // Arrange
        var input =
            @"trait Person {
    var name: String
}";

        // Act
        var parseResult = CodecParser.ParseTrait.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Person");
        result.Fields.Count.ShouldBe(1);

        var nameField = result.Fields.FirstOrDefault(f => f.Name == "name");
        nameField.ShouldNotBeNull();
        nameField.Type.ShouldBe(TypeInfo.String);
        nameField.IsMutable.ShouldBeTrue();
    }

    [Fact]
    public void ParseTrait_Empty_ShouldParseCorrectly()
    {
        // Arrange
        var input =
            @"trait Empty {
}";

        // Act
        var parseResult = CodecParser.ParseTrait.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Empty");
        result.Fields.Count.ShouldBe(0);
    }

    [Theory]
    [InlineData("trait lowercase { }", "Trait names must use PascalCase")]
    [InlineData("trait { }", "Missing trait name")]
    [InlineData("traitNamed { }", "Missing space after keyword")]
    public void ParseTrait_InvalidSyntax_ShouldFail(string input, string reason)
    {
        // Act
        var parseResult = CodecParser.ParseTrait.Parse(input);

        // Assert
        parseResult.Success.ShouldBeFalse($"Should fail for: {reason}");
    }
}
