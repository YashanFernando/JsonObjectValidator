using System.Diagnostics.CodeAnalysis;

namespace JsonObjectValidator;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public sealed class JsonValidationException : Exception
{
    /// <summary>
    /// The location of the property or item that failed validation
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The actual JSON value that caused the exception
    /// </summary>
    public string? ActualObject { get; }

    internal JsonValidationException(string message, string path, string? actualObject, Exception? innerException = null)
        : base(message, innerException)
    {
        Path = path;
        ActualObject = actualObject;

        // Add values to the data so that it's displayed in the error string
        Data[nameof(Path)] = path;
        Data[nameof(ActualObject)] = actualObject;
    }
}