using Microsoft.Data.Sqlite;

namespace Pos.App.Storage.Db;

public class SqliteDb
{
    public string ConnectionString { get; }

    public SqliteDb(string dbPath)
    {
        ConnectionString = $"Data Source={dbPath}";
    }

    public SqliteConnection OpenConnection()
        => new SqliteConnection(ConnectionString);
}
