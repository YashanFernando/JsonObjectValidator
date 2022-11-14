using System.Collections;
using System.Text.Json;
using ExpectedObjects;

namespace JsonObjectValidator;

public static class JsonMatcher
{
    /// <summary>
    /// Execute a custom comparison
    /// </summary>
    /// <param name="expectation">A function that evaluates the value and returns a boolean</param>
    /// <typeparam name="T">Type of the field</typeparam>
    public static Expectation<T> Expect<T>(Func<T, bool> expectation) => new(expectation);

    /// <summary>
    /// Verify a field of type exists
    /// </summary>
    /// <typeparam name="T">Type of the field</typeparam>
    public static Expectation<T> ExpectAny<T>() => new(_ => true);

    /// <summary>
    /// Verify the field exists and it is null
    /// </summary>
    public static Expectation<JsonElement> ExpectNull() => new(e => e.ValueKind == JsonValueKind.Null);

    /// <summary>
    /// Perform an unordered list comparison
    /// </summary>
    /// <param name="list">The list to compare</param>
    /// <returns></returns>
    /// <remarks>Expectations cannot be used within this list</remarks>
    public static Expectation<T> ExpectUnorderedList<T>(T list) where T : IEnumerable
    {
        return new Expectation<T>(json => list.ToExpectedObject().Matches(json));
    }
}