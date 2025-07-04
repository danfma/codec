using Codec.Core.AST;
using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class SchemaParsingTests
{
    [Fact]
    public void ParseTypeAlias_FromSchema_ShouldWork()
    {
        // Test just the first type alias from the schema
        var input = "type Email = String { }";

        var parseResult = CodecParser.ParseTypeAlias.Parse(input);

        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Name.ShouldBe("Email");
        parseResult.Value.UnderlyingType.ShouldBe(TypeInfo.String);
    }

    [Fact]
    public void ParseTrait_FromSchema_ShouldWork()
    {
        // Test the trait from the schema
        var input = "trait Named { var name: String }";

        var parseResult = CodecParser.ParseTrait.Parse(input);

        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Name.ShouldBe("Named");
        parseResult.Value.Fields.Count.ShouldBe(1);
    }

    [Fact]
    public void ParseEntity_Empty_ShouldWork()
    {
        // Test empty entity first
        var input = "entity Individual { }";

        var parseResult = CodecParser.ParseEntity.Parse(input);

        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Name.ShouldBe("Individual");
        parseResult.Value.ImplementedTraits.Length.ShouldBe(0);
        parseResult.Value.Fields.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseEntity_FromSchema_ShouldWork()
    {
        // Test a simple entity from the schema without inheritance
        var input = "entity Individual { }";

        var parseResult = CodecParser.ParseEntity.Parse(input);

        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Name.ShouldBe("Individual");
        parseResult.Value.ImplementedTraits.Length.ShouldBe(0);
        parseResult.Value.Fields.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseEntity_WithInheritance_ShouldWork()
    {
        // Test entity with inheritance
        var input = "entity Individual : Named { }";

        var parseResult = CodecParser.ParseEntity.Parse(input);

        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Name.ShouldBe("Individual");
        parseResult.Value.ImplementedTraits.Length.ShouldBe(1);
        parseResult.Value.ImplementedTraits[0].Name.ShouldBe("Named");
        parseResult.Value.Fields.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseAbstractEntity_FromSchema_ShouldWork()
    {
        // Test the abstract entity from the schema
        var input = "abstract entity Person : Named { }";

        var parseResult = CodecParser.ParseAbstractEntity.Parse(input);

        parseResult.Success.ShouldBeTrue($"Failed to parse: {input}");
        parseResult.Value.Name.ShouldBe("Person");
        parseResult.Value.ImplementedTraits.Length.ShouldBe(1);
        parseResult.Value.ImplementedTraits[0].Name.ShouldBe("Named");
        parseResult.Value.Fields.Length.ShouldBe(0);
    }
}
