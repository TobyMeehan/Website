using OneOf;
using TobyMeehan.Com.Results;

namespace TobyMeehan.Com;

public static class OneOfExtensions
{
    public static bool IsNotFound<T>(this OneOf<T, NotFound> oneOf)
    {
        return oneOf.IsT1;
    }

    public static bool IsNotFound<T0, T1>(this OneOf<T0, T1, NotFound> oneOf)
    {
        return oneOf.IsT2;
    }

    public static bool IsSuccess<T, T1>(this OneOf<T, T1> oneOf, out T result)
    {
        return oneOf.TryPickT0(out result, out _);
    }

    public static bool IsSuccess<T, T1, T2>(this OneOf<T, T1, T2> oneOf, out T result)
    {
        return oneOf.TryPickT0(out result, out _);
    }
}