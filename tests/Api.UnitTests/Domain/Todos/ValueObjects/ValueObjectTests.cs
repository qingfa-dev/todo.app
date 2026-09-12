using Shouldly;

using Todo.Api.Domain.Common;

namespace Todo.Api.UnitTests.Domain.Todos.ValueObjects;

public class ValueObjectTests
{
    private sealed class TestValueObject(string value) : ValueObject
    {
        public string Value { get; } = value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

    private sealed class AnotherValueObject(string value) : ValueObject
    {
        public string Value { get; } = value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

    #region Equals

    [Fact]
    public void Equals_WhenSameTypeAndSameComponents_ShouldBeEqual()
    {
        var vo1 = new TestValueObject("test");
        var vo2 = new TestValueObject("test");

        vo1.Equals(vo2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_WhenSameTypeAndDifferentComponents_ShouldNotBeEqual()
    {
        var vo1 = new TestValueObject("test1");
        var vo2 = new TestValueObject("test2");

        vo1.Equals(vo2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WhenDifferentType_ShouldNotBeEqual()
    {
        var vo1 = new TestValueObject("test");
        var vo2 = new AnotherValueObject("test");

        vo1.Equals(vo2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WhenNull_ShouldNotBeEqual()
    {
        var vo1 = new TestValueObject("test");

        vo1.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_WhenSameReference_ShouldBeEqual()
    {
        var vo1 = new TestValueObject("test");
        TestValueObject vo2 = vo1;

        vo1.Equals(vo2).ShouldBeTrue();
    }

    #endregion

    #region GetHashCode

    [Fact]
    public void GetHashCode_WhenSameComponents_ShouldBeEqual()
    {
        var vo1 = new TestValueObject("test");
        var vo2 = new TestValueObject("test");

        vo1.GetHashCode().ShouldBe(vo2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WhenDifferentComponents_ShouldNotBeEqual()
    {
        var vo1 = new TestValueObject("test1");
        var vo2 = new TestValueObject("test2");

        vo1.GetHashCode().ShouldNotBe(vo2.GetHashCode());
    }

    #endregion

    #region Operators

    [Fact]
    public void OperatorEquals_WhenSameComponents_ShouldBeTrue()
    {
        var vo1 = new TestValueObject("test");
        var vo2 = new TestValueObject("test");

        (vo1 == vo2).ShouldBeTrue();
    }

    [Fact]
    public void OperatorEquals_WhenDifferentComponents_ShouldBeFalse()
    {
        var vo1 = new TestValueObject("test1");
        var vo2 = new TestValueObject("test2");

        (vo1 == vo2).ShouldBeFalse();
    }

    [Fact]
    public void OperatorNotEquals_WhenSameComponents_ShouldBeFalse()
    {
        var vo1 = new TestValueObject("test");
        var vo2 = new TestValueObject("test");

        (vo1 != vo2).ShouldBeFalse();
    }

    [Fact]
    public void OperatorNotEquals_WhenDifferentComponents_ShouldBeTrue()
    {
        var vo1 = new TestValueObject("test1");
        var vo2 = new TestValueObject("test2");

        (vo1 != vo2).ShouldBeTrue();
    }

    #endregion
}
