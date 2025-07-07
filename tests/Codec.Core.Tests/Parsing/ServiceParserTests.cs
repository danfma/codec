using Codec.Core.Parsing;

namespace Codec.Core.Tests.Parsing;

public class ServiceParserTests
{
    [Fact]
    public void ParseService_ShouldParseSimpleService()
    {
        // Arrange
        var input = "service PersonService { }";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.Services.Length.ShouldBe(1);
        var service = result.Services[0];
        
        service.Name.ShouldBe("PersonService");
        service.Functions.ShouldBeEmpty();
    }

    [Fact]
    public void ParseService_ShouldParseServiceWithWhitespace()
    {
        // Arrange
        var input = "  service   PersonService   {   }  ";

        // Act
        var result = CodecParser.Parse(input);

        // Assert
        result.Services.Length.ShouldBe(1);
        var service = result.Services[0];
        
        service.Name.ShouldBe("PersonService");
        service.Functions.ShouldBeEmpty();
    }

    [Fact]
    public void ParseService_ShouldReturnEmptyProjectWithInvalidServiceName()
    {
        // Arrange
        var input = "service personService { }"; // lowercase name

        // Act
        var result = CodecParser.Parse(input);
        
        // Assert - Parser is now tolerant
        result.ShouldNotBeNull();
        result.Services.Length.ShouldBe(0);
    }

    [Fact]
    public void ParseService_ShouldReturnEmptyProjectWithMissingBraces()
    {
        // Arrange
        var input = "service PersonService";

        // Act
        var result = CodecParser.Parse(input);
        
        // Assert - Parser is now tolerant
        result.ShouldNotBeNull();
        result.Services.Length.ShouldBe(0);
    }
}