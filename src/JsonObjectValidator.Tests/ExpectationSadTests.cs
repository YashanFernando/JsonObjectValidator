using System.Text.Json;

namespace JsonObjectValidator.Tests;

public class ExpectationSadTests
{
    [Test]
    public void ExpectNull()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": 1 }"
                .JsonShouldLookLike(new
            {
                TestProperty = JsonMatcher.ExpectNull()
            });
        });
    }

    [Test]
    public void ExpectInvalidType()
    {
        var exception = Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": 5 }"
                .JsonShouldLookLike(new
            {
                TestProperty = JsonMatcher.ExpectAny<string>()
            });
        });

        Assert.IsInstanceOf<JsonException>(exception!.InnerException);
    }

    [Test]
    public void ExpectInt()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": 5 }"
                .JsonShouldLookLike(new
            {
                TestProperty = JsonMatcher.Expect<int>(val => val == 6)
            });
        });
    }

    [Test]
    public void ExpectNullableValues()
    {
        var exception = Assert.Throws<JsonValidationException>(() =>
        {
            "[ { \"TestProperty\": null }, { \"TestProperty\": 5 } ]"
                .JsonShouldLookLike(new[]
            {
                new
                {
                    TestProperty = JsonMatcher.Expect<int?>(val => val is not null)
                },
                new
                {
                    TestProperty = JsonMatcher.Expect<int?>(val => val == 5)
                }
            });
        });

        Assert.That(exception!.Path, Is.EqualTo("ExpectedObject[0].TestProperty"));
    }

    [Test]
    public void ExpectUnorderedList()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "[ 1, 2, 3 ]"
                .JsonShouldLookLike(JsonMatcher.ExpectUnorderedList(new[]
            {
                3, 2, 4
            }));
        });
    }
}