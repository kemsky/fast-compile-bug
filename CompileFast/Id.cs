namespace CompileFast.Identity;

public readonly struct Id<T>
{
    private readonly long _value;

    public Id(long value)
    {
        _value = value;
    }

    public static implicit operator long?(Id<T> d) => d._value;
}