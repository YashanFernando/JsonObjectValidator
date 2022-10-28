namespace JsonObjectValidator;

internal static class TypeExtensions
{
    public static bool IsAnonymousObject(this Type type) =>
        type.IsClass && type.IsSealed && !type.IsPublic &&
        type.Name.Contains("AnonymousType", StringComparison.InvariantCulture);

    public static bool IsExpectation(this Type type) =>
        type.IsClass && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expectation<>);
}