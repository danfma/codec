using Codec.Core.AST;
using Codec.Core.Parsing;

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
        var result = CodecParser.Parse(input);

        // Assert
        result.Traits.Length.ShouldBe(1);
        var trait = result.Traits[0];

        trait.Name.ShouldBe("Named");
        trait.Fields.Count.ShouldBe(1);

        var field = trait.Fields.First();
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
        var result = CodecParser.Parse(input);

        // Assert
        result.Traits.Length.ShouldBe(1);
        var trait = result.Traits[0];

        trait.Name.ShouldBe("Person");
        trait.Fields.Count.ShouldBe(1);

        var nameField = trait.Fields.FirstOrDefault(f => f.Name == "name");
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
        var result = CodecParser.Parse(input);

        // Assert
        result.Traits.Length.ShouldBe(1);
        var trait = result.Traits[0];

        trait.Name.ShouldBe("Empty");
        trait.Fields.Count.ShouldBe(0);
    }

    [Theory]
    [InlineData("trait lowercase { }", "Trait names must use PascalCase")]
    [InlineData("trait { }", "Missing trait name")]
    [InlineData("traitNamed { }", "Missing space after keyword")]
    public void ParseTrait_InvalidSyntax_ShouldReturnEmptyProject(string input, string reason)
    {
        // Act
        var result = CodecParser.Parse(input);
        
        // Assert - Parser is now tolerant and returns empty/partial results
        result.ShouldNotBeNull();
        result.Traits.Length.ShouldBe(0);
    }
}