using System.Text.Json;

namespace Validator;

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
}