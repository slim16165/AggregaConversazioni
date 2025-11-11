using Xunit;
using FluentAssertions;
using AggregaConversazioni.Application.Transformers;

namespace AggregaConversazioni.UnitTests.Transformers;

public class MessengerTransformerTests
{
    [Fact]
    public void Transform_ValidMessengerFormat_ReturnsWikiFormat()
    {
        // Arrange
        var transformer = new MessengerTransformer();
        var input = "[10:00 AM] John: Ciao, come stai?\n[10:01 AM] Jane: Bene, grazie!";

        // Act
        var result = transformer.Transform(input);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotContain("10:00 AM"); // Timestamp rimosso
    }

    [Fact]
    public void Transform_EmptyInput_ReturnsEmptyString()
    {
        // Arrange
        var transformer = new MessengerTransformer();
        var input = string.Empty;

        // Act
        var result = transformer.Transform(input);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Transform_YouSent_ReplacedWithIo()
    {
        // Arrange
        var transformer = new MessengerTransformer();
        var input = "You sent: Ciao!";

        // Act
        var result = transformer.Transform(input);

        // Assert
        result.Should().Contain("Io:");
    }
}
