using System.Collections;
using System.Data.Common;

namespace Garoa.Tests.Mapping;

/// <summary>
/// A minimal in-memory <see cref="DbDataReader"/> used to drive the mapper with precise
/// control over the CLR types a "provider" returns — including DateOnly/TimeOnly, which is
/// exactly the scenario that needs first-class support. It relies on the base
/// <see cref="DbDataReader.GetFieldValue{T}(int)"/> implementation (a typed cast over
/// <see cref="GetValue"/>), mirroring how a real provider hands typed values to Garoa.
/// </summary>
internal sealed class FakeDataReader : DbDataReader
{
    private readonly string[] _columns;
    private readonly Type[] _types;
    private readonly IReadOnlyList<object?[]> _rows;
    private int _row = -1;

    public FakeDataReader(string[] columns, Type[] types, IReadOnlyList<object?[]> rows)
    {
        _columns = columns;
        _types = types;
        _rows = rows;
    }

    public override int FieldCount => _columns.Length;
    public override bool HasRows => _rows.Count > 0;
    public override bool IsClosed => false;
    public override int Depth => 0;
    public override int RecordsAffected => -1;

    public override bool Read()
    {
        _row++;
        return _row < _rows.Count;
    }

    public override bool NextResult() => false;

    public override string GetName(int ordinal) => _columns[ordinal];

    public override int GetOrdinal(string name)
    {
        for (int i = 0; i < _columns.Length; i++)
            if (string.Equals(_columns[i], name, StringComparison.OrdinalIgnoreCase))
                return i;
        throw new IndexOutOfRangeException(name);
    }

    public override Type GetFieldType(int ordinal) => _types[ordinal];

    public override string GetDataTypeName(int ordinal) => _types[ordinal].Name;

    public override object GetValue(int ordinal) => _rows[_row][ordinal] ?? DBNull.Value;

    public override bool IsDBNull(int ordinal) => _rows[_row][ordinal] is null;

    public override int GetValues(object[] values)
    {
        int n = Math.Min(values.Length, FieldCount);
        for (int i = 0; i < n; i++)
            values[i] = GetValue(i);
        return n;
    }

    public override object this[int ordinal] => GetValue(ordinal);
    public override object this[string name] => GetValue(GetOrdinal(name));

    // Typed accessors delegate to the boxed value — sufficient for tests.
    public override bool GetBoolean(int ordinal) => (bool)GetValue(ordinal);
    public override byte GetByte(int ordinal) => (byte)GetValue(ordinal);
    public override char GetChar(int ordinal) => (char)GetValue(ordinal);
    public override DateTime GetDateTime(int ordinal) => (DateTime)GetValue(ordinal);
    public override decimal GetDecimal(int ordinal) => (decimal)GetValue(ordinal);
    public override double GetDouble(int ordinal) => (double)GetValue(ordinal);
    public override float GetFloat(int ordinal) => (float)GetValue(ordinal);
    public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);
    public override short GetInt16(int ordinal) => (short)GetValue(ordinal);
    public override int GetInt32(int ordinal) => (int)GetValue(ordinal);
    public override long GetInt64(int ordinal) => (long)GetValue(ordinal);
    public override string GetString(int ordinal) => (string)GetValue(ordinal);

    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        => throw new NotSupportedException();

    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        => throw new NotSupportedException();

    public override IEnumerator GetEnumerator() => throw new NotSupportedException();
}
