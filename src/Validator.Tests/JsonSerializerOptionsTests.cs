using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Validator.Tests;

public class JsonSerializerOptionsTests
{
    [Test]
    public void CustomSerializationOptions()
    {
        "{ \"testProperty\": \"FirstValue\" }"
            .JsonShouldLookLike(new
        {
            TestProperty = TestEnum.FirstValue,
        }, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
    }

    [Test]
    public void CustomSerializationOptionsWithNestedObjects()
    {
        "{ \"testClass\": { \"testProperty\": \"SecondValue\" } }"
            .JsonShouldLookLike(new
        {
            TestClass = new
            {
                TestProperty = TestEnum.SecondValue,
            }
        }, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
    }

    [Test]
    public void VerifyJsonNode()
    {
        JsonSerializer
            .Deserialize<JsonNode>("{ \"InnerObject\": { \"TestProperty\": 15 } }")!
            ["InnerObject"]?.ToJsonString()
            .JsonShouldLookLike(new
            {
                TestProperty = 15,
            });
    }

    private enum TestEnum
    {
        FirstValue,
        SecondValue
    }
}