using Pos.App.Core.Domain;

namespace Pos.App.Core.Services;

public class PosSessionService : IPosSessionService
{
    private PosSession? _current;

    public Task<bool> StartSessionAsync(PosSession session, CancellationToken ct = default)
    {
        _current = session;
        return Task.FromResult(true);
    }

    public PosSession? Current => _current;
}
