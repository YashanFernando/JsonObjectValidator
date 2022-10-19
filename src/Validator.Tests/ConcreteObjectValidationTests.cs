namespace Validator.Tests;

public class ConcreteTypeValidationTests
{
    [Test]
    public void NestedObjectWithProperty()
    {
        // Technically, we do support concrete classes as long as
        // they support equality comparisons

        "{ \"TestProperty\": { \"InnerInt\": 5 } }"
            .JsonShouldLookLike(new
        {
            TestProperty = new TestObject
            {
                InnerInt = 5
            }
        });
    }

    [Test]
    public void NestedObjectAsExpectation()
    {
        "{ \"TestProperty\": { \"InnerInt\": 5 } }"
            .JsonShouldLookLike(new
        {
            TestProperty = JsonMatcher.ExpectValue(new TestObject
            {
                InnerInt = 5
            })
        });
    }

    private class TestObject
    {
        public int InnerInt { get; init; }

        private bool Equals(TestObject other)
        {
            return InnerInt == other.InnerInt;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestObject) obj);
        }

        public override int GetHashCode()
        {
            return InnerInt;
        }
    }
}