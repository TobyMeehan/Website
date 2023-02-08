using System.Diagnostics.CodeAnalysis;

namespace TobyMeehan.Com;

public struct Optional<T>
{
    public Optional()
    {
        
    }
    
    private T? _value = default;
    
    public T? Value
    {
        get => _value;
        set
        {
            _value = value;
            _isChanged = true;
        }
    }

    private bool _isChanged = false;
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsChanged => _isChanged;

    public static implicit operator Optional<T>(T value) => new() {Value = value};

    public static T operator |(Optional<T> option, T value)
    {
        return option.IsChanged ? option.Value : value;
    }
}