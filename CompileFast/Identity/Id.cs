using System.Diagnostics;

namespace CompileFast.Identity;

// some further reading https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/
public interface IId
{
    long GetValue();
}

[DebuggerDisplay("{DebuggerDisplay}")]
public readonly struct Id<T> : IComparable<Id<T>>, IEquatable<Id<T>>, IId
{
    private readonly long _value;

    public Id(long value)
    {
        _value = value;
    }

    long IId.GetValue()
    {
        return _value;
    }

    public int CompareTo(Id<T> other)
    {
        return _value.CompareTo(other._value);
    }

    public bool Equals(Id<T> other)
    {
        return _value.Equals(other._value);
    }

    public override bool Equals(object obj)
    {
        return obj is Id<T> id && _value.Equals(id._value);
    }

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString();

    public static implicit operator long(Id<T> d) => d._value;

    public static implicit operator long?(Id<T> d) => d._value;

    public static implicit operator long?(Id<T>? d) => d?._value;

    public static implicit operator Id<T>(long b) => new Id<T>(b);

    public static implicit operator Id<T>?(long b) => new Id<T>(b);

    public static implicit operator Id<T>?(long? b) => b.HasValue ? new Id<T>(b.Value) : null;

    public static bool operator ==(Id<T>? a, Id<T>? b) => a.HasValue && b.HasValue && a.Value._value == b.Value._value || !a.HasValue && !b.HasValue;

    public static bool operator !=(Id<T>? a, Id<T>? b) => !(a == b);

    public static bool operator ==(Id<T> a, Id<T> b) => a._value == b._value;

    public static bool operator !=(Id<T> a, Id<T> b) => !(a == b);

    private string DebuggerDisplay => $"Id<{typeof(T).Name}>({_value})";
}