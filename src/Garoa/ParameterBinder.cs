using System.Collections;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Garoa;

/// <summary>
/// Binds the public readable properties of a parameter object to a command's parameters.
/// Property <c>Name</c> maps to a parameter named <c>Name</c>; reference SQL with the
/// provider's marker (e.g. <c>@Name</c>). <see langword="null"/> values become <see cref="DBNull"/>.
/// This is deliberately not a <c>DynamicParameters</c> equivalent — just plain anonymous objects.
/// </summary>
/// <remarks>
/// A property whose value is a non-string, non-<c>byte[]</c> <see cref="IEnumerable"/> is expanded for
/// <c>IN</c> clauses: the <c>@Name</c> token in the SQL becomes <c>(@Name0, @Name1, …)</c> and one
/// parameter is added per element. This is a deliberately small token substitution for cross-provider
/// <c>IN @ids</c> support (PostgreSQL also offers <c>= ANY(@ids)</c> with a native array, which avoids
/// expansion entirely), not a SQL parser. An empty sequence becomes a guaranteed-false predicate
/// (<c>(SELECT 1 WHERE 1=0)</c>) so a valid query returning no rows is emitted instead of <c>IN ()</c>.
/// </remarks>
internal static class ParameterBinder
{
    public static void Bind(DbCommand command, object? parameters)
    {
        if (parameters is null)
            return;

        foreach (PropertyInfo prop in parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || prop.GetIndexParameters().Length > 0)
                continue;

            object? value = prop.GetValue(parameters);

            // A list value (but not a string or byte[], which are scalar) expands into an IN list.
            if (value is IEnumerable enumerable && value is not string && value is not byte[])
            {
                BindList(command, prop.Name, enumerable);
                continue;
            }

            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = prop.Name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }

    // Expands @name into (@name0, @name1, …) and adds one parameter per element, or a false predicate
    // when the sequence is empty (IN () is a syntax error on every provider).
    private static void BindList(DbCommand command, string name, IEnumerable values)
    {
        var placeholders = new List<string>();
        int index = 0;
        foreach (object? item in values)
        {
            string elementName = name + index.ToString(CultureInfo.InvariantCulture);
            placeholders.Add("@" + elementName);

            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = elementName;
            parameter.Value = item ?? DBNull.Value;
            command.Parameters.Add(parameter);
            index++;
        }

        string replacement = placeholders.Count == 0
            ? "(SELECT 1 WHERE 1=0)"
            : "(" + string.Join(", ", placeholders) + ")";

        command.CommandText = ReplaceToken(command.CommandText, name, replacement);
    }

    // Replaces the @name token wherever it appears, matching it as a whole word (so @name never matches
    // @names) and never the trailing @ of @@name. Case-insensitive, matching how providers resolve
    // parameter names. A MatchEvaluator is used so '$' in the replacement is treated literally.
    private static string ReplaceToken(string sql, string name, string replacement)
    {
        string pattern = @"(?<!@)@" + Regex.Escape(name) + @"(?![A-Za-z0-9_])";
        return Regex.Replace(sql, pattern, _ => replacement, RegexOptions.IgnoreCase);
    }
}
