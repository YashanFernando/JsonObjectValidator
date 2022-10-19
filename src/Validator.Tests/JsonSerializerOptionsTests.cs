using System.Text.Json;
using System.Text.Json.Nodes;

namespace Validator.Tests;

public class JsonSerializerOptionsTests
{
    [Test]
    public void CustomSerializationOptions()
    {
        "{ \"testProperty\": 15 }"
            .JsonShouldLookLike(new
        {
            TestProperty = 15,
        }, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    [Test]
    public void VerifyJsonNode()
    {
        JsonSerializer
            .Deserialize<JsonNode>("{ \"InnerObject\": { \"TestProperty\": 15 } }")!
            ["InnerObject"]
            .JsonShouldLookLike(new
            {
                TestProperty = 15,
            });
    }
}