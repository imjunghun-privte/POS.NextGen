using Pos.App.Core.Domain;
using Pos.App.Core.Services;

namespace Pos.App.Core.UseCases;

public class OpenPosSession
{
    private readonly IPosSessionService _sessionService;

    public OpenPosSession(IPosSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public Task<bool> ExecuteAsync(string storeId, string posId, CancellationToken ct = default)
    {
        var session = new PosSession
        {
            StoreId = storeId,
            PosId = posId,
            StartedAt = DateTime.UtcNow
        };

        return _sessionService.StartSessionAsync(session, ct);
    }
}
