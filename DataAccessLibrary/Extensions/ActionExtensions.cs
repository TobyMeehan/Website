using System;

namespace TobyMeehan.Com.Data.Extensions;

public static class ActionExtensions
{
    public static T Apply<T>(this Action<T> configure, T value)
    {
        configure(value);

        return value;
    }
}