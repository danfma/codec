using Codec.Core.Parsing;
using Pidgin;

namespace Codec.Core.Tests.Parsing;

public class ServiceParserTests
{
    [Fact]
    public void ParseService_ShouldParseSimpleService()
    {
        // Arrange
        var input = "service PersonService { }";

        // Act
        var result = CodecParser.ParseService.ParseOrThrow(input);

        // Assert
        result.Name.ShouldBe("PersonService");
        result.Functions.ShouldBeEmpty();
    }

    [Fact]
    public void ParseService_ShouldParseServiceWithWhitespace()
    {
        // Arrange
        var input = "  service   PersonService   {   }  ";

        // Act
        var result = CodecParser.ParseService.ParseOrThrow(input);

        // Assert
        result.Name.ShouldBe("PersonService");
        result.Functions.ShouldBeEmpty();
    }

    [Fact]
    public void ParseService_ShouldFailWithInvalidServiceName()
    {
        // Arrange
        var input = "service personService { }"; // lowercase name

        // Act & Assert
        var parseResult = CodecParser.ParseService.Parse(input);
        parseResult.Success.ShouldBeFalse();
    }

    [Fact]
    public void ParseService_ShouldFailWithMissingBraces()
    {
        // Arrange
        var input = "service PersonService";

        // Act & Assert
        var parseResult = CodecParser.ParseService.Parse(input);
        parseResult.Success.ShouldBeFalse();
    }
}
