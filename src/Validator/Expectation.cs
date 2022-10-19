﻿using System.Diagnostics;

namespace Validator;

[StackTraceHidden]
public class Expectation<T>
{
    private readonly Func<T, bool> _expectation;

    /// <summary>
    /// Json value expectation
    /// </summary>
    /// <param name="expectation">A function that evaluates the value and returns a boolean</param>
    /// <typeparam name="T">Type of the field</typeparam>
    public Expectation(Func<T, bool> expectation)
    {
        _expectation = expectation;
    }

    /// <summary>
    /// For internal use only. Used to validate the expectation.
    /// </summary>
    internal bool Verify(T input) => _expectation(input);
}