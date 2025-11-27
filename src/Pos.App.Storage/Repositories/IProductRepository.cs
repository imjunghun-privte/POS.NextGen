using Pos.App.Core.Domain;

namespace Pos.App.Storage.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
}
