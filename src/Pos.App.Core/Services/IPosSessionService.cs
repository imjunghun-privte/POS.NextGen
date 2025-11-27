using Pos.App.Core.Domain;

namespace Pos.App.Core.Services;

public interface IPosSessionService
{
    Task<bool> StartSessionAsync(PosSession session, CancellationToken ct = default);
}
