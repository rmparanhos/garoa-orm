using System.Data.Common;
using Garoa.Mapping;
using Xunit;

namespace Garoa.Tests.Mapping;

public class MapperTests
{
    private sealed class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public TimeOnly? WakeUpTime { get; set; }
        public int? ManagerId { get; set; }
        public Status Status { get; set; }

        [Column("years_at_company")]
        public int Tenure { get; set; }
    }

    private enum Status
    {
        Inactive = 0,
        Active = 1,
    }

    private static T MapSingle<T>(string[] columns, Type[] types, object?[] row)
    {
        var reader = new FakeDataReader(columns, types, new[] { row });
        Assert.True(reader.Read());
        Func<DbDataReader, T> map = MapperFactory.Create<T>(columns);
        return map(reader);
    }

    [Fact]
    public void Maps_DateOnly_and_TimeOnly_straight_through_the_provider()
    {
        var birth = new DateOnly(1990, 5, 17);
        var wake = new TimeOnly(6, 30);

        Person person = MapSingle<Person>(
            new[] { "Id", "Name", "BirthDate", "WakeUpTime" },
            new[] { typeof(int), typeof(string), typeof(DateOnly), typeof(TimeOnly) },
            new object?[] { 1, "Ada", birth, wake });

        Assert.Equal(1, person.Id);
        Assert.Equal("Ada", person.Name);
        Assert.Equal(birth, person.BirthDate);
        Assert.Equal(wake, person.WakeUpTime);
    }

    [Fact]
    public void Maps_snake_case_columns_and_Column_attribute()
    {
        Person person = MapSingle<Person>(
            new[] { "years_at_company" },
            new[] { typeof(int) },
            new object?[] { 7 });

        Assert.Equal(7, person.Tenure);
    }

    [Fact]
    public void Handles_nulls_for_nullable_members()
    {
        Person person = MapSingle<Person>(
            new[] { "Name", "WakeUpTime", "ManagerId" },
            new[] { typeof(string), typeof(TimeOnly), typeof(int) },
            new object?[] { null, null, null });

        Assert.Null(person.Name);
        Assert.Null(person.WakeUpTime);
        Assert.Null(person.ManagerId);
    }

    [Fact]
    public void Leaves_default_for_null_on_non_nullable_value_type()
    {
        Person person = MapSingle<Person>(
            new[] { "Id" },
            new[] { typeof(int) },
            new object?[] { null });

        Assert.Equal(0, person.Id);
    }

    [Fact]
    public void Maps_enum_from_numeric_column()
    {
        Person person = MapSingle<Person>(
            new[] { "Status" },
            new[] { typeof(int) },
            new object?[] { 1 });

        Assert.Equal(Status.Active, person.Status);
    }

    [Fact]
    public void Maps_scalar_result()
    {
        int value = MapSingle<int>(
            new[] { "count" },
            new[] { typeof(int) },
            new object?[] { 42 });

        Assert.Equal(42, value);
    }

    [Fact]
    public void Maps_nullable_scalar_null_to_null()
    {
        int? value = MapSingle<int?>(
            new[] { "maybe" },
            new[] { typeof(int) },
            new object?[] { null });

        Assert.Null(value);
    }

    [Fact]
    public void Mapping_error_names_the_offending_column()
    {
        // BirthDate is the second column; supplying a string where a DateOnly is expected must
        // blame BirthDate — not the previously-read column.
        var ex = Assert.Throws<GaroaMappingException>(() => MapSingle<Person>(
            new[] { "Id", "BirthDate" },
            new[] { typeof(int), typeof(DateOnly) },
            new object?[] { 1, "not-a-date" }));

        Assert.Contains("BirthDate", ex.Message);
        Assert.Contains("ordinal 1", ex.Message);
    }

    [Fact]
    public void Unmatched_columns_are_ignored()
    {
        Person person = MapSingle<Person>(
            new[] { "Id", "totally_unknown_column" },
            new[] { typeof(int), typeof(string) },
            new object?[] { 5, "ignored" });

        Assert.Equal(5, person.Id);
    }
}
