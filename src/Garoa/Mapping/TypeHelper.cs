namespace Garoa.Mapping;

internal static class TypeHelper
{
    /// <summary>
    /// A "simple" type maps directly from a single column (scalar) rather than member-by-member.
    /// Covers primitives, enums, strings and the common BCL value types including the
    /// date/time-only types that motivated Garoa in the first place.
    /// </summary>
    public static bool IsSimpleType(Type type)
    {
        Type t = Nullable.GetUnderlyingType(type) ?? type;

        if (t.IsPrimitive || t.IsEnum)
            return true;

        if (t == typeof(string)
            || t == typeof(decimal)
            || t == typeof(DateTime)
            || t == typeof(DateTimeOffset)
            || t == typeof(TimeSpan)
            || t == typeof(Guid)
            || t == typeof(DateOnly)
            || t == typeof(TimeOnly)
            || t == typeof(byte[])
            || t == typeof(object))
            return true;

        return false;
    }

    /// <summary>
    /// Normalises a column or member name for matching: lower-cased and underscore-stripped,
    /// so <c>user_id</c>, <c>UserId</c> and <c>userid</c> all collapse to the same key.
    /// </summary>
    public static string NormalizeName(string name)
    {
        if (name.IndexOf('_') < 0)
            return name.ToLowerInvariant();

        return string.Create(CountWithout(name, '_'), name, static (span, src) =>
        {
            int i = 0;
            foreach (char c in src)
            {
                if (c == '_') continue;
                span[i++] = char.ToLowerInvariant(c);
            }
        });
    }

    private static int CountWithout(string value, char skip)
    {
        int count = 0;
        foreach (char c in value)
            if (c != skip) count++;
        return count;
    }
}
