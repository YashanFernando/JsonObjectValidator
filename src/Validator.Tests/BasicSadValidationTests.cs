using System.Text.Json;

namespace Validator.Tests;

public class BasicSadValidationTests
{
    [Test]
    public void StringProperty()
    {
        var exception = Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": \"TestValue\" }"
                .JsonShouldLookLike(new
                {
                    TestProperty = "NotTestValue"
                });
        });

        Assert.That(exception!.Path, Is.EqualTo("ExpectedObject.TestProperty"));
    }

    [Test]
    public void IntProperty()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": 10 }"
                .JsonShouldLookLike(new
            {
                TestProperty = 11
            });
        });
    }

    [Test]
    public void NullJsonValue()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": null }"
                .JsonShouldLookLike(new
            {
                TestProperty = new int?(2)
            });
        });
    }

    [Test]
    public void NullExpectedValue()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": 10 }"
                .JsonShouldLookLike(new
            {
                TestProperty = null as int?
            });
        });
    }

    [Test]
    public void OneOfMultipleProperties()
    {
        var exception = Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"FirstProperty\": \"TestValue\", \"SecondProperty\": 2 }"
                .JsonShouldLookLike(new
            {
                FirstProperty = "TestValue",
                SecondProperty = 3
            });
        });

        Assert.That(exception!.Path, Is.EqualTo("ExpectedObject.SecondProperty"));
    }

    [Test]
    public void IntArray()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "[ 10, 5, 6 ]".JsonShouldLookLike(new[] { 10, 5, 7 });
        });
    }

    [Test]
    public void ObjectArray()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "[ { \"TestProperty\": 1 }, { \"TestProperty\": 2 } ]".JsonShouldLookLike(new[]
            {
                new
                {
                    TestProperty = 1
                },
                new
                {
                    TestProperty = 3
                }
            });
        });
    }

    [Test]
    public void ExpectLess()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "[ 10, 5, 6 ]".JsonShouldLookLike(new[] { 10, 5 });
        });
    }

    [Test]
    public void ExpectMore()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "[ 10, 5, 6 ]".JsonShouldLookLike(new[] { 10, 5, 6, 1 });
        });
    }

    [Test]
    public void InvalidArrayItem()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": [ 10, 5, 6 ] }"
                .JsonShouldLookLike(new
            {
                TestProperty = new[] { 10, 5, 7 }
            });
        });
    }

    [Test]
    public void NestedObjectWithProperty()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": { \"InnerValue\": 5 } }"
                .JsonShouldLookLike(new
            {
                TestProperty = new
                {
                    InnerValue = 6
                }
            });
        });
    }

    [Test]
    public void NestedObjectWithArray()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": { \"InnerValue\":  [5, 4, 3] } }"
                .JsonShouldLookLike(new
            {
                TestProperty = new
                {
                    InnerValue = new[] { 5, 4, 2 }
                }
            });
        });
    }

    [Test]
    public void MultipleNestedObjects()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": { \"SecondaryValue\": { \"TertiaryValue\": 5 } } }"
                .JsonShouldLookLike(new
            {
                TestProperty = new
                {
                    SecondaryValue = new
                    {
                        TertiaryValue = 3
                    }
                }
            });
        });
    }

    [Test]
    public void NestedObjectArray()
    {
        Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": [ { \"SecondaryValue\": 3 }, { \"SecondaryValue\": 5 } ] }"
                .JsonShouldLookLike(new
            {
                TestProperty = new[]
                {
                    new
                    {
                        SecondaryValue = 3
                    },
                    new
                    {
                        SecondaryValue = 4
                    }
                }
            });
        });
    }

    [Test]
    public void NestedArrayOfArrays()
    {
        var exception = Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": [ { \"SecondaryValue\": [ 3, 5, 6 ] } ] }"
                .JsonShouldLookLike(new
            {
                TestProperty = new[]
                {
                    new
                    {
                        SecondaryValue = new[] { 3, 5, 7 }
                    }
                }
            });
        });

        Assert.That(exception!.Path, Is.EqualTo("ExpectedObject.TestProperty[0].SecondaryValue[2]"));
    }

    [Test]
    public void ExpectInvalidType()
    {
        var exception = Assert.Throws<JsonValidationException>(() =>
        {
            "{ \"TestProperty\": 5 }"
                .JsonShouldLookLike(new
            {
                TestProperty = "Five"
            });
        });

        Assert.That(exception!.Path, Is.EqualTo("ExpectedObject.TestProperty"));
        Assert.IsInstanceOf<JsonException>(exception.InnerException);
    }
}