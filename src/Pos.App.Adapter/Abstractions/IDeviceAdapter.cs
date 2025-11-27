namespace Pos.App.Adapter.Abstractions;

public interface IDeviceAdapter
{
    Task<bool> InitializeAsync(CancellationToken ct = default);
}
