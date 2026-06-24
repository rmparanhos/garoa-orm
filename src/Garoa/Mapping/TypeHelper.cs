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

    /// <summary>
    /// Converts a PascalCase/camelCase member name to <c>snake_case</c>: an underscore is inserted
    /// before each upper-case letter that starts a new word (i.e. follows a lower-case letter or
    /// digit, or begins a word after an acronym run), then the whole string is lower-cased. So
    /// <c>BirthDate</c> → <c>birth_date</c>, <c>ManagerId</c> → <c>manager_id</c>,
    /// <c>HTTPStatus</c> → <c>http_status</c>.
    /// </summary>
    public static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var sb = new System.Text.StringBuilder(name.Length + 8);
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (char.IsUpper(c))
            {
                bool prevIsWordChar = i > 0 && (char.IsLower(name[i - 1]) || char.IsDigit(name[i - 1]));
                bool startsWordInAcronym = i > 0 && i + 1 < name.Length && char.IsLower(name[i + 1]);
                if (prevIsWordChar || startsWordInAcronym)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
