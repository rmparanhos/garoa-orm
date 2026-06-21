using Garoa.Bulk;
using Xunit;

namespace Garoa.Tests.Bulk;

public class BulkCoreTests
{
    private sealed class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public Status Status { get; set; }
        public Status? OptionalStatus { get; set; }

        [Column("years_at_company")]
        public int Tenure { get; set; }

        // Not writable -> must still be readable for bulk (it is), so it is included.
        public int Computed => Id + 1;
    }

    private enum Status
    {
        Inactive = 0,
        Active = 1,
    }

    [Fact]
    public void Explicit_columns_drive_order_and_selection()
    {
        BulkColumnSet<Person> set = BulkColumnSet<Person>.Get(new[] { "Name", "Id" });

        Assert.Equal(new[] { "Name", "Id" }, set.ColumnNames);

        var buffer = new object?[set.Count];
        set.Fill(new Person { Id = 7, Name = "Ada" }, buffer);

        Assert.Equal("Ada", buffer[0]);
        Assert.Equal(7, buffer[1]);
    }

    [Fact]
    public void Column_attribute_is_honoured_for_matching()
    {
        BulkColumnSet<Person> set = BulkColumnSet<Person>.Get(new[] { "years_at_company" });

        var buffer = new object?[set.Count];
        set.Fill(new Person { Tenure = 12 }, buffer);

        Assert.Equal(12, buffer[0]);
    }

    [Fact]
    public void Enums_are_written_as_their_numeric_value()
    {
        BulkColumnSet<Person> set = BulkColumnSet<Person>.Get(new[] { "Status", "OptionalStatus" });

        var buffer = new object?[set.Count];
        set.Fill(new Person { Status = Status.Active, OptionalStatus = null }, buffer);

        Assert.Equal(1, buffer[0]);
        Assert.IsType<int>(buffer[0]);
        Assert.Null(buffer[1]);

        Assert.Equal(typeof(int), set.ColumnTypes[0]);
    }

    [Fact]
    public void Unknown_column_throws_a_clear_error()
    {
        GaroaMappingException ex = Assert.Throws<GaroaMappingException>(
            () => BulkColumnSet<Person>.Get(new[] { "does_not_exist" }));

        Assert.Contains("does_not_exist", ex.Message);
    }

    [Fact]
    public void ObjectDataReader_streams_rows_without_materialising()
    {
        BulkColumnSet<Person> set = BulkColumnSet<Person>.Get(new[] { "Id", "Name" });
        var people = new[]
        {
            new Person { Id = 1, Name = "Ada" },
            new Person { Id = 2, Name = null },
        };

        using var reader = new ObjectDataReader<Person>(people, set);

        Assert.Equal(2, reader.FieldCount);
        Assert.Equal("Id", reader.GetName(0));

        Assert.True(reader.Read());
        Assert.Equal(1, reader.GetValue(0));
        Assert.Equal("Ada", reader.GetValue(1));
        Assert.False(reader.IsDBNull(1));

        Assert.True(reader.Read());
        Assert.Equal(2, reader.GetValue(0));
        Assert.True(reader.IsDBNull(1));
        Assert.Equal(DBNull.Value, reader.GetValue(1));

        Assert.False(reader.Read());
    }

    [Fact]
    public void ObjectDataReader_pulls_lazily_from_the_source()
    {
        BulkColumnSet<Person> set = BulkColumnSet<Person>.Get(new[] { "Id" });
        int produced = 0;

        IEnumerable<Person> Source()
        {
            while (true)
            {
                produced++;
                yield return new Person { Id = produced };
            }
        }

        using var reader = new ObjectDataReader<Person>(Source(), set);

        reader.Read();
        reader.Read();

        // An infinite source proves nothing is buffered up-front: only two items were pulled.
        Assert.Equal(2, produced);
    }
}
