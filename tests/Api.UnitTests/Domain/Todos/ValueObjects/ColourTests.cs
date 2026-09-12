using Shouldly;

using Todo.Api.Domain.Todos.Exceptions;
using Todo.Api.Domain.Todos.ValueObjects;

namespace Todo.Api.UnitTests.Domain.Todos.ValueObjects;

public class ColourTests
{
    #region From

    [Fact]
    public void From_WhenSupportedColour_ShouldReturnColour()
    {
        var colour = Colour.From("#E05C4D");

        colour.Code.ShouldBe("#E05C4D");
    }

    [Theory]
    [InlineData("#E05C4D")]
    [InlineData("#D98B2B")]
    [InlineData("#4CAF50")]
    [InlineData("#26A69A")]
    [InlineData("#5C6BC0")]
    [InlineData("#AB47BC")]
    [InlineData("#78909C")]
    public void From_WhenAllSupportedColours_ShouldReturnColour(string code)
    {
        var colour = Colour.From(code);

        colour.Code.ShouldBe(code);
    }

    [Theory]
    [InlineData("#000000")]
    [InlineData("#FF0000")]
    [InlineData("")]
    [InlineData("invalid")]
    public void From_WhenUnsupportedColour_ShouldThrowUnsupportedColourException(string code)
    {
        Should.Throw<UnsupportedColourException>(() => Colour.From(code));
    }

    #endregion

    #region Static properties

    [Fact]
    public void Red_ShouldHaveCorrectCode()
    {
        Colour.Red.Code.ShouldBe("#E05C4D");
    }

    [Fact]
    public void Orange_ShouldHaveCorrectCode()
    {
        Colour.Orange.Code.ShouldBe("#D98B2B");
    }

    [Fact]
    public void Green_ShouldHaveCorrectCode()
    {
        Colour.Green.Code.ShouldBe("#4CAF50");
    }

    [Fact]
    public void Teal_ShouldHaveCorrectCode()
    {
        Colour.Teal.Code.ShouldBe("#26A69A");
    }

    [Fact]
    public void Blue_ShouldHaveCorrectCode()
    {
        Colour.Blue.Code.ShouldBe("#5C6BC0");
    }

    [Fact]
    public void Purple_ShouldHaveCorrectCode()
    {
        Colour.Purple.Code.ShouldBe("#AB47BC");
    }

    [Fact]
    public void Grey_ShouldHaveCorrectCode()
    {
        Colour.Grey.Code.ShouldBe("#78909C");
    }

    #endregion

    #region SupportedColours

    [Fact]
    public void SupportedColours_ShouldContainAllSevenColours()
    {
        Colour.SupportedColours.Count().ShouldBe(7);
    }

    [Fact]
    public void SupportedColours_ShouldContainAllExpectedColours()
    {
        var supported = Colour.SupportedColours.ToList();

        supported.ShouldContain(c => c.Code == "#E05C4D");
        supported.ShouldContain(c => c.Code == "#D98B2B");
        supported.ShouldContain(c => c.Code == "#4CAF50");
        supported.ShouldContain(c => c.Code == "#26A69A");
        supported.ShouldContain(c => c.Code == "#5C6BC0");
        supported.ShouldContain(c => c.Code == "#AB47BC");
        supported.ShouldContain(c => c.Code == "#78909C");
    }

    #endregion

    #region Implicit operator

    [Fact]
    public void ImplicitConversionToString_ShouldReturnCode()
    {
        Colour colour = Colour.Red;

        string code = colour;

        code.ShouldBe("#E05C4D");
    }

    #endregion

    #region Explicit operator

    [Fact]
    public void ExplicitConversionFromString_ShouldReturnColour()
    {
        Colour colour = (Colour)"#4CAF50";

        colour.Code.ShouldBe("#4CAF50");
    }

    [Fact]
    public void ExplicitConversionFromUnsupportedString_ShouldThrow()
    {
        Should.Throw<UnsupportedColourException>(() => (Colour)"#000000");
    }

    #endregion

    #region ToString

    [Fact]
    public void ToString_ShouldReturnCode()
    {
        Colour colour = Colour.Blue;

        colour.ToString().ShouldBe("#5C6BC0");
    }

    #endregion

    #region Equality

    [Fact]
    public void Equals_WhenSameCode_ShouldBeEqual()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Red;

        colour1.ShouldBe(colour2);
    }

    [Fact]
    public void Equals_WhenDifferentCode_ShouldNotBeEqual()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Blue;

        colour1.ShouldNotBe(colour2);
    }

    [Fact]
    public void GetHashCode_WhenSameCode_ShouldBeEqual()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Red;

        colour1.GetHashCode().ShouldBe(colour2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WhenDifferentCode_ShouldNotBeEqual()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Blue;

        colour1.GetHashCode().ShouldNotBe(colour2.GetHashCode());
    }

    [Fact]
    public void OperatorEquals_WhenSameCode_ShouldBeTrue()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Red;

        (colour1 == colour2).ShouldBeTrue();
    }

    [Fact]
    public void OperatorEquals_WhenDifferentCode_ShouldBeFalse()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Blue;

        (colour1 == colour2).ShouldBeFalse();
    }

    [Fact]
    public void OperatorNotEquals_WhenSameCode_ShouldBeFalse()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Red;

        (colour1 != colour2).ShouldBeFalse();
    }

    [Fact]
    public void OperatorNotEquals_WhenDifferentCode_ShouldBeTrue()
    {
        Colour colour1 = Colour.Red;
        Colour colour2 = Colour.Blue;

        (colour1 != colour2).ShouldBeTrue();
    }

    #endregion

    #region Constructor

    [Fact]
    public void Constructor_WithNullCode_ShouldDefaultToBlack()
    {
        var colour = new Colour(null!);

        colour.Code.ShouldBe("#000000");
    }

    [Fact]
    public void Constructor_WithEmptyCode_ShouldDefaultToBlack()
    {
        var colour = new Colour(string.Empty);

        colour.Code.ShouldBe("#000000");
    }

    [Fact]
    public void Constructor_WithWhitespaceCode_ShouldDefaultToBlack()
    {
        var colour = new Colour("   ");

        colour.Code.ShouldBe("#000000");
    }

    [Fact]
    public void Constructor_WithValidCode_ShouldSetCode()
    {
        var colour = new Colour("#E05C4D");

        colour.Code.ShouldBe("#E05C4D");
    }

    #endregion
}
