using System.Diagnostics.CodeAnalysis;

namespace Validator;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
public class JsonValidationException : Exception
{
    /// <summary>
    /// The location of the property or item that failed validation
    /// </summary>
    public string Path { get; }

    internal JsonValidationException(string message, string path)
        : base($"{message} at {path}")
    {
        Path = path;
    }

    internal JsonValidationException(string message, string path, Exception innerException)
        : base($"{message} at {path}", innerException)
    {
        Path = path;
    }
}