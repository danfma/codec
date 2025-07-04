using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class EntityParserTests
{
    [Fact]
    public void ParseEntity_SimpleEntity_ShouldParseCorrectly()
    {
        // Arrange
        var input = "entity Individual { }"; // Simplified to empty entity

        // Act
        var parseResult = CodecParser.ParseEntity.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Individual");
        result.BaseType.ShouldBeNull();
        result.ImplementedTraits.Length.ShouldBe(0);
        result.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Fact]
    public void ParseEntity_WithTraitInheritance_ShouldParseCorrectly()
    {
        // Arrange
        var input = "entity Individual : Named { }";

        // Act
        var parseResult = CodecParser.ParseEntity.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Individual");
        result.BaseType.ShouldBeNull();
        result.ImplementedTraits.Length.ShouldBe(1);
        result.ImplementedTraits[0].Name.ShouldBe("Named");
        result.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Fact]
    public void ParseAbstractEntity_ShouldParseCorrectly()
    {
        // Arrange
        var input = "abstract entity Person : Named { }";

        // Act
        var parseResult = CodecParser.ParseAbstractEntity.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Person");
        result.ImplementedTraits.Length.ShouldBe(1);
        result.ImplementedTraits[0].Name.ShouldBe("Named");
        result.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Fact]
    public void ParseEntity_WithEntityInheritance_ShouldParseCorrectly()
    {
        // Arrange - This would require having Person already parsed as AbstractEntityDefinition
        var input = "entity Individual : Person { }";

        // Act
        var parseResult = CodecParser.ParseEntity.Parse(input);

        // Assert
        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");

        var result = parseResult.Value;

        result.Name.ShouldBe("Individual");
        // For now, we'll parse Person as a trait until we implement proper entity inheritance resolution
        result.ImplementedTraits.Length.ShouldBe(1);
        result.ImplementedTraits[0].Name.ShouldBe("Person");
        result.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Theory]
    [InlineData("entity lowercase { }", "Entity names must use PascalCase")]
    [InlineData("entity { }", "Missing entity name")]
    [InlineData("entityIndividual { }", "Missing space after keyword")]
    public void ParseEntity_InvalidSyntax_ShouldFail(string input, string reason)
    {
        // Act
        var parseResult = CodecParser.ParseEntity.Parse(input);

        // Assert
        parseResult.Success.ShouldBeFalse($"Should fail for: {reason}");
    }
}
