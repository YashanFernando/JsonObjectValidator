namespace Validator.Tests;

public class BasicHappyValidationTests
{
    [Test]
    public void EmptyObject()
    {
        "{ \"TestProperty\": \"TestValue\" }"
            .JsonShouldLookLike(new { });
    }

    [Test]
    public void StringProperty()
    {
        "{ \"TestProperty\": \"TestValue\" }"
            .JsonShouldLookLike(new
        {
            TestProperty = "TestValue"
        });
    }

    [Test]
    public void IntProperty()
    {
        "{ \"TestProperty\": 10 }"
            .JsonShouldLookLike(new
        {
            TestProperty = 10
        });
    }

    [Test]
    public void NullProperty()
    {
        "{ \"TestProperty\": null }"
            .JsonShouldLookLike(new
        {
            // This will work but it's nicer to use JsonMatcher.ExpectNull() instead
            TestProperty = null as string
        });
    }

    [Test]
    public void NewValueTypeProperty()
    {
        "{ \"TestProperty\": 2 }"
            .JsonShouldLookLike(new
        {
            TestProperty = new int?(2)
        });
    }

    [Test]
    public void MultipleProperties()
    {
        "{ \"FirstProperty\": \"TestValue\", \"SecondProperty\": 2 }"
            .JsonShouldLookLike(new
        {
            FirstProperty = "TestValue",
            SecondProperty = 2
        });
    }

    [Test]
    public void IntArray()
    {
        "[ 10, 5, 6 ]".JsonShouldLookLike(new[] { 10, 5, 6 });
    }

    [Test]
    public void ObjectArray()
    {
        "[ { \"TestProperty\": 1 }, { \"TestProperty\": 2 } ]".JsonShouldLookLike(new[]
        {
            new
            {
                TestProperty = 1
            },
            new
            {
                TestProperty = 2
            }
        });
    }

    [Test]
    public void IntArrayProperty()
    {
        "{ \"TestProperty\": [ 10, 5, 6 ] }"
            .JsonShouldLookLike(new
        {
            TestProperty = new[] { 10, 5, 6 }
        });
    }

    [Test]
    public void NestedObjectWithProperty()
    {
        "{ \"TestProperty\": { \"InnerValue\": 5 } }"
            .JsonShouldLookLike(new
        {
            TestProperty = new
            {
                InnerValue = 5
            }
        });
    }

    [Test]
    public void NestedObjectWithArray()
    {
        "{ \"TestProperty\": { \"InnerValue\":  [ 5, 4, 3 ] } }"
            .JsonShouldLookLike(new
        {
            TestProperty = new
            {
                InnerValue = new[] { 5, 4, 3 }
            }
        });
    }

    [Test]
    public void MultipleNestedObjects()
    {
        "{ \"TestProperty\": { \"SecondaryValue\": { \"TertiaryValue\": 5 } } }"
            .JsonShouldLookLike(new
        {
            TestProperty = new
            {
                SecondaryValue = new
                {
                    TertiaryValue = 5
                }
            }
        });
    }

    [Test]
    public void NestedObjectArray()
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
                    SecondaryValue = 5
                }
            }
        });
    }

    [Test]
    public void NestedArrayOfArrays()
    {
        "{ \"TestProperty\": [ { \"SecondaryValue\": [ 3, 5, 6 ] } ] }"
            .JsonShouldLookLike(new
        {
            TestProperty = new[]
            {
                new
                {
                    SecondaryValue = new[] { 3, 5, 6 }
                }
            }
        });
    }

    [Test]
    public void PartialComparison()
    {
        // Verify only a part of the JSON

        "{ \"TestedProperty\": \"TestedValue\", \"UntestedProperty\": 5 }"
            .JsonShouldLookLike(new
        {
            TestedProperty = "TestedValue"
        });
    }

    [Test]
    public void IntList()
    {
        "[ 10, 5, 6 ]".JsonShouldLookLike(new List<int> { 10, 5, 6 });
    }

    [Test]
    public void ObjectList()
    {
        "[ { \"TestProperty\": 1 }, { \"TestProperty\": 2 } ]".JsonShouldLookLike(new List<object>
        {
            new
            {
                TestProperty = 1
            },
            new
            {
                TestProperty = 2
            }
        });
    }
}