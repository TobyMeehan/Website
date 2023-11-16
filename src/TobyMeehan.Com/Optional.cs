using System.Diagnostics.CodeAnalysis;

namespace TobyMeehan.Com;

public readonly struct Optional<T>
{
    public static Optional<T> Empty() => new();

    public static Optional<T> Of(T value) => new(value);

    private readonly T? _value = default;
    private readonly bool _hasValue = false;

    private Optional(T value)
    {
        _value = value;
        _hasValue = true;
    }
    
    public T Value
    {
        get
        {
            if (!HasValue)
            {
                throw new InvalidOperationException("Optional value is not set.");
            }

            return _value!;
        }
    }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => _hasValue;
    public bool IsEmpty => !_hasValue;

    public T ValueOr(T value)
    {
        return HasValue ? Value : value;
    }

    public T ValueOr(Func<T> valueFunc)
    {
        return HasValue ? Value : valueFunc();
    }

    public Optional<TMap> Map<TMap>(Func<T, TMap> map)
    {
        return HasValue ? map(Value) : Optional<TMap>.Empty();
    }

    public TMap MapOr<TMap>(Func<T, TMap> map, TMap other)
    {
        return HasValue ? map(Value) : other;
    }

    public TMap MapOr<TMap>(Func<T, TMap> map, Func<TMap> otherFunc)
    {
        return HasValue ? map(Value) : otherFunc();
    }

    public static implicit operator Optional<T>(T value) => new(value);

    public static T operator |(Optional<T> option, T value)
    {
        return option.ValueOr(value);
    }

    public static T operator |(Optional<T> option, Func<T> valueFunc)
    {
        return option.ValueOr(valueFunc);
    }

    public override bool Equals(object? obj)
    {
        return obj is Optional<T> optional && Equals(optional);
    }

    public bool Equals(Optional<T> optional)
    {
        if (HasValue && optional.HasValue)
        {
            return Value.Equals(optional.Value);
        }

        return IsEmpty && optional.IsEmpty;
    }

    public override int GetHashCode()
    {
        return HasValue ? Value.GetHashCode() : _hasValue.GetHashCode();
    }
}