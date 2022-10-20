namespace Validator.Tests;

public class ExpectationHappyTests
{
    [Test]
    public void ExpectNull()
    {
        "{ \"TestProperty\": null }"
            .JsonShouldLookLike(new
        {
            TestProperty = JsonMatcher.ExpectNull()
        });
    }

    [Test]
    public void ExpectAnyInt()
    {
        "{ \"TestProperty\": 5 }"
            .JsonShouldLookLike(new
        {
            TestProperty = JsonMatcher.ExpectAny<int>()
        });
    }

    [Test]
    public void ExpectNullableValues()
    {
        "[ { \"TestProperty\": null }, { \"TestProperty\": 5 } ]"
            .JsonShouldLookLike(new[]
        {
            new
            {
                TestProperty = JsonMatcher.Expect<int?>(val => val is null)
            },
            new
            {
                TestProperty = JsonMatcher.ExpectValue<int?>(5)
            }
        });
    }

    [Test]
    public void ExpectInt()
    {
        "{ \"TestProperty\": 5 }"
            .JsonShouldLookLike(new
        {
            TestProperty = JsonMatcher.Expect<int>(val => val == 5)
        });
    }

    [Test]
    public void ExpectPartialValidation()
    {
        "{ \"TestProperty\": 15, \"ExpectedProperty\": 5 }"
            .JsonShouldLookLike(new
        {
            TestProperty = 15,
            ExpectedProperty = JsonMatcher.Expect<int>(val => val is > 4 and < 15)
        });
    }
}