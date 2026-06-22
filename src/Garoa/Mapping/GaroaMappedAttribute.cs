namespace Garoa;

/// <summary>
/// Opts a type into <b>compile-time</b> mapping. When the <c>Garoa</c> source generator sees a
/// type annotated with this attribute, it emits a specialised <see cref="IGaroaRowMapper{T}"/> at
/// build time — no runtime expression-tree compilation, typed reader getters where possible, and
/// full Native AOT / trimming compatibility.
/// </summary>
/// <remarks>
/// Mapping semantics are identical to the runtime mapper: case- and underscore-insensitive
/// column matching, <see cref="ColumnAttribute"/> support, nullable and enum handling. Types
/// without this attribute continue to use the runtime expression-tree mapper, so the attribute is
/// purely opt-in. A public parameterless constructor is required; types without one are skipped by
/// the generator and fall back to the runtime mapper.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class GaroaMappedAttribute : Attribute
{
}
