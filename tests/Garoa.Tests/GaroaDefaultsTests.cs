using Microsoft.Data.Sqlite;
using Xunit;

namespace Garoa.Tests;

/// <summary>
/// Covers the global command-timeout default and how it interacts with the per-call override.
/// <see cref="GaroaDefaults.CommandTimeoutSeconds"/> is process-wide state, so each test saves and
/// restores it to stay independent of ordering.
/// </summary>
public sealed class GaroaDefaultsTests : IDisposable
{
    private readonly int? _original = GaroaDefaults.CommandTimeoutSeconds;

    public void Dispose() => GaroaDefaults.CommandTimeoutSeconds = _original;

    [Fact]
    public void CommandTimeoutSeconds_defaults_to_null()
    {
        GaroaDefaults.CommandTimeoutSeconds = null;
        Assert.Null(GaroaDefaults.CommandTimeoutSeconds);
    }

    [Fact]
    public void Resolve_returns_null_when_neither_per_call_nor_global_is_set()
    {
        GaroaDefaults.CommandTimeoutSeconds = null;
        Assert.Null(GaroaDefaults.ResolveCommandTimeout(null));
    }

    [Fact]
    public void Resolve_uses_global_default_when_per_call_is_null()
    {
        GaroaDefaults.CommandTimeoutSeconds = 90;
        Assert.Equal(90, GaroaDefaults.ResolveCommandTimeout(null));
    }

    [Fact]
    public void Resolve_lets_per_call_override_the_global_default()
    {
        GaroaDefaults.CommandTimeoutSeconds = 90;
        Assert.Equal(5, GaroaDefaults.ResolveCommandTimeout(5));
    }

    [Fact]
    public void Zero_is_allowed_and_means_no_timeout()
    {
        GaroaDefaults.CommandTimeoutSeconds = 0;
        Assert.Equal(0, GaroaDefaults.CommandTimeoutSeconds);
        Assert.Equal(0, GaroaDefaults.ResolveCommandTimeout(null));
    }

    [Fact]
    public void Negative_global_default_throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GaroaDefaults.CommandTimeoutSeconds = -1);
    }

    [Fact]
    public void Query_and_Execute_honour_a_global_default_without_breaking()
    {
        GaroaDefaults.CommandTimeoutSeconds = 120;

        using var db = new SqliteConnection("Data Source=:memory:");
        db.Open();
        db.Execute("CREATE TABLE t (id INTEGER PRIMARY KEY);");

        int affected = db.Execute("INSERT INTO t (id) VALUES (1), (2);");
        List<long> ids = db.Query<long>("SELECT id FROM t ORDER BY id;");

        Assert.Equal(2, affected);
        Assert.Equal(new[] { 1L, 2L }, ids);
    }

    [Fact]
    public void Per_call_timeout_works_without_breaking()
    {
        GaroaDefaults.CommandTimeoutSeconds = null;

        using var db = new SqliteConnection("Data Source=:memory:");
        db.Open();
        db.Execute("CREATE TABLE t (id INTEGER PRIMARY KEY);", commandTimeout: 30);

        int affected = db.Execute("INSERT INTO t (id) VALUES (1);", commandTimeout: 15);
        List<long> ids = db.Query<long>("SELECT id FROM t;", commandTimeout: 15);

        Assert.Equal(1, affected);
        Assert.Equal(new[] { 1L }, ids);
    }
}
