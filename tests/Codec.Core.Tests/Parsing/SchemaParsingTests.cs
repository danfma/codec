using Codec.Core.AST;
using Codec.Core.Parsing;

namespace Codec.Core.Tests.Parsing;

public class SchemaParsingTests
{
    [Fact]
    public void ParseTypeAlias_FromSchema_ShouldWork()
    {
        // Test just the first type alias from the schema
        var input = "type Email = String { }";

        var result = CodecParser.Parse(input);

        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];
        
        typeAlias.Name.ShouldBe("Email");
        typeAlias.UnderlyingType.ShouldBe(TypeInfo.String);
    }

    [Fact]
    public void ParseTrait_FromSchema_ShouldWork()
    {
        // Test the trait from the schema
        var input = "trait Named { var name: String }";

        var result = CodecParser.Parse(input);

        result.Traits.Length.ShouldBe(1);
        var trait = result.Traits[0];
        
        trait.Name.ShouldBe("Named");
        trait.Fields.Count.ShouldBe(1);
    }

    [Fact]
    public void ParseEntity_Empty_ShouldWork()
    {
        // Test empty entity first
        var input = "entity Individual { }";

        var result = CodecParser.Parse(input);

        result.Entities.Length.ShouldBe(1);
        var entity = result.Entities[0];
        
        entity.Name.ShouldBe("Individual");
        entity.ImplementedTraits.Length.ShouldBe(0);
        entity.Fields.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseEntity_FromSchema_ShouldWork()
    {
        // Test a simple entity from the schema without inheritance
        var input = "entity Individual { }";

        var result = CodecParser.Parse(input);

        result.Entities.Length.ShouldBe(1);
        var entity = result.Entities[0];
        
        entity.Name.ShouldBe("Individual");
        entity.ImplementedTraits.Length.ShouldBe(0);
        entity.Fields.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseEntity_WithInheritance_ShouldWork()
    {
        // Test entity with inheritance
        var input = "entity Individual : Named { }";

        var result = CodecParser.Parse(input);

        result.Entities.Length.ShouldBe(1);
        var entity = result.Entities[0];
        
        entity.Name.ShouldBe("Individual");
        entity.ImplementedTraits.Length.ShouldBe(1);
        entity.ImplementedTraits[0].Name.ShouldBe("Named");
        entity.Fields.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseAbstractEntity_FromSchema_ShouldWork()
    {
        // Test the abstract entity from the schema
        var input = "abstract entity Person : Named { }";

        var result = CodecParser.Parse(input);

        result.AbstractEntities.Length.ShouldBe(1);
        var abstractEntity = result.AbstractEntities[0];
        
        abstractEntity.Name.ShouldBe("Person");
        abstractEntity.ImplementedTraits.Length.ShouldBe(1);
        abstractEntity.ImplementedTraits[0].Name.ShouldBe("Named");
        abstractEntity.Fields.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseCompleteSchema_ShouldWork()
    {
        // Test parsing multiple top-level statements together
        var input = @"
package Test.Package;

type Email = String { }

trait Named {
    var name: String
}

abstract entity Person : Named { }

entity Individual : Person { }

service PersonService { }
";

        var result = CodecParser.Parse(input);

        result.PackageName.ShouldBe("Test.Package");
        result.TypeAliases.Length.ShouldBe(1);
        result.Traits.Length.ShouldBe(1);
        result.AbstractEntities.Length.ShouldBe(1);
        result.Entities.Length.ShouldBe(1);
        result.Services.Length.ShouldBe(1);
        
        result.TypeAliases[0].Name.ShouldBe("Email");
        result.Traits[0].Name.ShouldBe("Named");
        result.AbstractEntities[0].Name.ShouldBe("Person");
        result.Entities[0].Name.ShouldBe("Individual");
        result.Services[0].Name.ShouldBe("PersonService");
    }
}