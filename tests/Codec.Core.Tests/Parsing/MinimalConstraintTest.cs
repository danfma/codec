using Codec.Core.AST;
using Codec.Core.Parsing;

namespace Codec.Core.Tests.Parsing;

public class MinimalConstraintTest
{
    [Fact]
    public void ParseSingleConstraint_MinLen_ShouldWork()
    {
        // Test constraint parsing through type alias
        var input = "type Test = String { constraint min_len(1) }";

        var result = CodecParser.Parse(input);

        result.TypeAliases.Length.ShouldBe(1);
        var typeAlias = result.TypeAliases[0];
        
        typeAlias.Name.ShouldBe("Test");
        typeAlias.Annotations.Length.ShouldBe(1);
        typeAlias.Annotations[0].ShouldBeOfType<Min>();
        
        var minConstraint = (Min)typeAlias.Annotations[0];
        minConstraint.Value.ShouldBe(1);
    }
}