using Dapper;
using Pos.App.Core.Domain;
using Pos.App.Storage.Db;

namespace Pos.App.Storage.Repositories;

public class SqliteProductRepository : IProductRepository
{
    private readonly SqliteDb _db;

    public SqliteProductRepository(SqliteDb db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        await using var conn = _db.OpenConnection();
        var rows = await conn.QueryAsync<Product>("SELECT Sku, Name, Price FROM Product");
        return rows.ToList();
    }
}
