using System.Collections;
using System.Data.Common;

namespace Garoa.Bulk;

/// <summary>
/// A forward-only <see cref="DbDataReader"/> that streams an <see cref="IEnumerable{T}"/> one
/// row at a time, exposing each item's values through a <see cref="BulkColumnSet{T}"/>. The
/// sequence is never materialised — only a single row buffer is held — which is what lets
/// bulk insert handle millions of rows without ballooning memory.
/// </summary>
internal sealed class ObjectDataReader<T> : DbDataReader
{
    private readonly IEnumerator<T> _source;
    private readonly BulkColumnSet<T> _columns;
    private readonly object?[] _buffer;
    private bool _open = true;

    public ObjectDataReader(IEnumerable<T> source, BulkColumnSet<T> columns)
    {
        _source = source.GetEnumerator();
        _columns = columns;
        _buffer = new object?[columns.Count];
    }

    public override int FieldCount => _columns.Count;
    public override bool HasRows => true;
    public override bool IsClosed => !_open;
    public override int Depth => 0;
    public override int RecordsAffected => -1;

    public override bool Read()
    {
        if (!_source.MoveNext())
            return false;

        _columns.Fill(_source.Current, _buffer);
        return true;
    }

    public override bool NextResult() => false;

    public override string GetName(int ordinal) => _columns.ColumnNames[ordinal];

    public override int GetOrdinal(string name)
    {
        string[] names = _columns.ColumnNames;
        for (int i = 0; i < names.Length; i++)
            if (string.Equals(names[i], name, StringComparison.OrdinalIgnoreCase))
                return i;
        throw new IndexOutOfRangeException(name);
    }

    public override Type GetFieldType(int ordinal) => _columns.ColumnTypes[ordinal];

    public override string GetDataTypeName(int ordinal) => _columns.ColumnTypes[ordinal].Name;

    public override object GetValue(int ordinal) => _buffer[ordinal] ?? DBNull.Value;

    public override bool IsDBNull(int ordinal) => _buffer[ordinal] is null;

    public override int GetValues(object[] values)
    {
        int n = Math.Min(values.Length, FieldCount);
        for (int i = 0; i < n; i++)
            values[i] = GetValue(i);
        return n;
    }

    public override object this[int ordinal] => GetValue(ordinal);
    public override object this[string name] => GetValue(GetOrdinal(name));

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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _open = false;
            _source.Dispose();
        }

        base.Dispose(disposing);
    }
}
