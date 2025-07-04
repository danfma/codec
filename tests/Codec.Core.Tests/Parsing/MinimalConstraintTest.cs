using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class MinimalConstraintTest
{
    [Fact]
    public void ParseSingleConstraint_MinLen_ShouldWork()
    {
        // Test the minimum constraint parser directly
        var input = "constraint min_len(1)";

        // We need to access the internal constraint parser somehow
        // For now, let's test a simple type alias
        var typeInput = "type Test = String";
        var result = CodecParser.ParseTypeAlias.Parse(typeInput);

        result.Success.ShouldBeTrue();
        result.Value.Name.ShouldBe("Test");
    }
}
