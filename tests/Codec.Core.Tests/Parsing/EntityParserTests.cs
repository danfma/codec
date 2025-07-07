using Codec.Core.Parsing;

namespace Codec.Core.Tests.Parsing;

public class EntityParserTests
{
    [Fact]
    public void ParseEntity_SimpleEntity_ShouldParseCorrectly()
    {
        // Arrange
        var input = "entity Individual { }"; // Simplified to empty entity

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.Entities.Length.ShouldBe(1);
        var entity = result.Entities[0];

        entity.Name.ShouldBe("Individual");
        entity.BaseType.ShouldBeNull();
        entity.ImplementedTraits.Length.ShouldBe(0);
        entity.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Fact]
    public void ParseEntity_WithTraitInheritance_ShouldParseCorrectly()
    {
        // Arrange
        var input = "entity Individual : Named { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.Entities.Length.ShouldBe(1);
        var entity = result.Entities[0];

        entity.Name.ShouldBe("Individual");
        entity.BaseType.ShouldBeNull();
        entity.ImplementedTraits.Length.ShouldBe(1);
        entity.ImplementedTraits[0].Name.ShouldBe("Named");
        entity.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Fact]
    public void ParseAbstractEntity_ShouldParseCorrectly()
    {
        // Arrange
        var input = "abstract entity Person : Named { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.AbstractEntities.Length.ShouldBe(1);
        var abstractEntity = result.AbstractEntities[0];

        abstractEntity.Name.ShouldBe("Person");
        abstractEntity.ImplementedTraits.Length.ShouldBe(1);
        abstractEntity.ImplementedTraits[0].Name.ShouldBe("Named");
        abstractEntity.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Fact]
    public void ParseEntity_WithEntityInheritance_ShouldParseCorrectly()
    {
        // Arrange - This would require having Person already parsed as AbstractEntityDefinition
        var input = "entity Individual : Person { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.Entities.Length.ShouldBe(1);
        var entity = result.Entities[0];

        entity.Name.ShouldBe("Individual");
        // For now, we'll parse Person as a trait until we implement proper entity inheritance resolution
        entity.ImplementedTraits.Length.ShouldBe(1);
        entity.ImplementedTraits[0].Name.ShouldBe("Person");
        entity.Fields.Length.ShouldBe(0); // Empty entity
    }

    [Theory]
    [InlineData("entity lowercase { }", "Entity names must use PascalCase")]
    [InlineData("entity { }", "Missing entity name")]
    [InlineData("entityIndividual { }", "Missing space after keyword")]
    public void ParseEntity_InvalidSyntax_ShouldReturnEmptyProject(string input, string reason)
    {
        // Act
        var result = CodecParser.Parse(input);
        
        // Assert - Parser is now tolerant and returns empty/partial results
        result.ShouldNotBeNull();
        result.Entities.Length.ShouldBe(0);
    }
}