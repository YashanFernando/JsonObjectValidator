using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Validator;

[StackTraceHidden]
public static class JsonValidator
{
    /// <summary>
    /// Compare a json object to an anonymous object
    /// </summary>
    /// <param name="json">Json string to validate</param>
    /// <param name="expectedObject">expression that returns an anonymous object to compare the JSON to</param>
    /// <param name="jsonSerializerOptions">[Optional] Options to control the behavior during parsing the JSON</param>
    /// <exception cref="JsonValidationException">Validation failure</exception>
    public static void JsonShouldLookLike<T>(this string json,
        T expectedObject,
        JsonSerializerOptions? jsonSerializerOptions = default) where T : class
    {
        JsonNode? actualObject = JsonSerializer.Deserialize<JsonNode>(json, jsonSerializerOptions);
        TraverseValue(expectedObject, actualObject, "ExpectedObject");
    }

    /// <summary>
    /// Compare a json object to an anonymous object
    /// </summary>
    /// <param name="jsonNode">JsonNode to validate</param>
    /// <param name="expectedObject">expression that returns an anonymous object to compare the JSON to</param>
    /// <exception cref="JsonValidationException">Validation failure</exception>
    public static void JsonShouldLookLike<T>(this JsonNode? jsonNode, T expectedObject) where T : class
    {
        TraverseValue(expectedObject, jsonNode, "ExpectedObject");
    }

    private static void TraverseValue(object? expectedObject, JsonNode? actualObject, string path)
    {
        if (expectedObject is null && actualObject is null)
            return;

        if (expectedObject is null)
            throw new JsonValidationException($"Expected null but actual is {actualObject?.ToJsonString()}", path);

        Type expectedObjectType = expectedObject.GetType();

        // Handle expectations
        if (expectedObjectType.IsExpectation())
        {
            RunExpectation(expectedObject, actualObject, path);
            return;
        }

        // We know the expected object is not null at this point so if the actualObject is null, they aren't equal.
        if (actualObject is null)
            throw new JsonValidationException("Expected a value but the actual is null", path);

        // Handle new anonymous object initializers
        if (expectedObjectType.IsClass
            && expectedObjectType.IsAnonymousObject())
        {
            TraverseObject(expectedObject, actualObject, path);
            return;
        }

        // Handle new array initializers
        if (expectedObjectType.IsArray)
        {
            Array? expectedArray = expectedObject as Array;
            JsonArray actualArray = actualObject.AsArray();

            if (expectedArray is null || actualArray is null)
                throw new InvalidOperationException();

            TraverseArray(expectedArray, actualArray, path);
            return;
        }

        // Check if the two objects/values are equal
        if (expectedObject.Equals(Deserialize(actualObject, expectedObjectType, path)))
        {
            return;
        }

        // Throw if the values don't match or if it's an unhandled expression type
        throw new JsonValidationException("Values don't match", path);
    }

    private static void TraverseObject(object expectedObject, JsonNode actualObject, string position)
    {
        Type expectedObjectType = expectedObject.GetType();
        var properties = expectedObjectType.GetProperties();

        // Iterate through the properties of the initializer of an anonymous object
        foreach (var property in properties)
        {
            object? expectedValue = property.GetValue(expectedObject);
            JsonNode? actualValue = actualObject[property.Name];

            TraverseValue(expectedValue, actualValue, string.Join('.', position, property.Name));
        }
    }

    private static void TraverseArray(Array expectedArray, JsonArray actualArray, string path)
    {
        if(expectedArray.Length != actualArray.Count)
            throw new JsonValidationException("Lists have different lengths", path);

        // Iterate through the initializer expressions inside initializer of the array
        for (int i = 0; i < expectedArray.Length; i++)
        {
            object? expected = expectedArray.GetValue(i);
            JsonNode? actual = actualArray[i];

            TraverseValue(expected, actual, $"{path}[{i}]");
        }
    }

    private static void RunExpectation(object expectedObject, JsonNode? actual, string path)
    {
        Type expectedObjectType = expectedObject.GetType();
        Type internalType = expectedObjectType.GenericTypeArguments[0];
        var actualValue = Deserialize(actual, internalType, path);

        MethodInfo verifyMethod = expectedObjectType.GetMethod(nameof(Expectation<string>.Verify),
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        bool successful = (bool) verifyMethod.Invoke(expectedObject, new[] { actualValue })!;

        if (!successful)
            throw new JsonValidationException("Expectation failed", path);
    }

    private static object? Deserialize(JsonNode? jsonNode, Type type, string path)
    {
        try
        {
            return jsonNode.Deserialize(type);
        }
        catch (JsonException e)
        {
            throw new JsonValidationException($"Couldn't deserialize into {type.Name}", path, e);
        }
    }
}