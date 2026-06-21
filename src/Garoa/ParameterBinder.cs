using System.Data.Common;
using System.Reflection;

namespace Garoa;

/// <summary>
/// Binds the public readable properties of a parameter object to a command's parameters.
/// Property <c>Name</c> maps to a parameter named <c>Name</c>; reference SQL with the
/// provider's marker (e.g. <c>@Name</c>). <see langword="null"/> values become <see cref="DBNull"/>.
/// This is deliberately not a <c>DynamicParameters</c> equivalent — just plain anonymous objects.
/// </summary>
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

            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = prop.Name;
            parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}
