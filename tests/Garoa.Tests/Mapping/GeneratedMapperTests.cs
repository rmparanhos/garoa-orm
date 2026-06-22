using System.Data.Common;
using Garoa.Mapping;
using Xunit;

namespace Garoa.Tests.Mapping;

// Marked for the source generator. Must be accessible (internal, top-level) so the generated
// mapper in the Garoa.Generated namespace can reference it.
[GaroaMapped]
internal sealed class GenPerson
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly BirthDate { get; set; }
    public TimeOnly? WakeUpTime { get; set; }
    public int? ManagerId { get; set; }
    public GenStatus Status { get; set; }

    [Column("years_at_company")]
    public int Tenure { get; set; }
}

internal enum GenStatus
{
    Inactive = 0,
    Active = 1,
}

/// <summary>
/// Exercises the source-generated mapper directly (no expression trees). Each test first asserts a
/// generated mapper was registered for the type — proving the generator ran — then maps a row
/// through it. Coverage mirrors <see cref="MapperTests"/> so the two paths stay behaviourally equal.
/// </summary>
public class GeneratedMapperTests
{
    private static T MapGenerated<T>(string[] columns, Type[] types, object?[] row)
    {
        IGaroaRowMapper<T>? mapper = GaroaGeneratedMappers.Get<T>();
        Assert.NotNull(mapper); // the generator emitted and registered a mapper for T

        var slots = new int[columns.Length];
        for (int i = 0; i < columns.Length; i++)
            slots[i] = mapper!.ResolveSlot(TypeHelper.NormalizeName(columns[i]));

        var reader = new FakeDataReader(columns, types, new[] { row });
        Assert.True(reader.Read());
        return mapper!.Map(reader, slots);
    }

    [Fact]
    public void Generator_registers_a_mapper_for_the_marked_type()
    {
        Assert.NotNull(GaroaGeneratedMappers.Get<GenPerson>());
    }

    [Fact]
    public void Maps_DateOnly_and_TimeOnly_straight_through_the_provider()
    {
        var birth = new DateOnly(1990, 5, 17);
        var wake = new TimeOnly(6, 30);

        GenPerson person = MapGenerated<GenPerson>(
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
        GenPerson person = MapGenerated<GenPerson>(
            new[] { "years_at_company" },
            new[] { typeof(int) },
            new object?[] { 7 });

        Assert.Equal(7, person.Tenure);
    }

    [Fact]
    public void Handles_nulls_for_nullable_members()
    {
        GenPerson person = MapGenerated<GenPerson>(
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
        GenPerson person = MapGenerated<GenPerson>(
            new[] { "Id" },
            new[] { typeof(int) },
            new object?[] { null });

        Assert.Equal(0, person.Id);
    }

    [Fact]
    public void Maps_enum_from_numeric_column()
    {
        GenPerson person = MapGenerated<GenPerson>(
            new[] { "Status" },
            new[] { typeof(int) },
            new object?[] { 1 });

        Assert.Equal(GenStatus.Active, person.Status);
    }

    [Fact]
    public void Mapping_error_names_the_offending_column()
    {
        var ex = Assert.Throws<GaroaMappingException>(() => MapGenerated<GenPerson>(
            new[] { "Id", "BirthDate" },
            new[] { typeof(int), typeof(DateOnly) },
            new object?[] { 1, "not-a-date" }));

        Assert.Contains("BirthDate", ex.Message);
        Assert.Contains("ordinal 1", ex.Message);
    }

    [Fact]
    public void Unmatched_columns_are_ignored()
    {
        GenPerson person = MapGenerated<GenPerson>(
            new[] { "Id", "totally_unknown_column" },
            new[] { typeof(int), typeof(string) },
            new object?[] { 5, "ignored" });

        Assert.Equal(5, person.Id);
    }
}
